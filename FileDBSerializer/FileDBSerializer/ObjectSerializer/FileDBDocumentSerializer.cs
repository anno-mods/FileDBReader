using FileDBSerializer.EncodingAwareStrings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace FileDBSerializing.ObjectSerializer
{
    //Loads an object structure into a FileDBDocument
    public class FileDBDocumentSerializer
    {
        private FileDBSerializerOptions Options;
        private IFileDBDocument TargetDocument;
        private PrimitiveTypeConverter PrimitiveConverter = new PrimitiveTypeConverter();

        #region Constructor
        public FileDBDocumentSerializer(FileDBSerializerOptions options)
        {
            Options = options;
            InitTargetDocument();
        }

        private void InitTargetDocument()
        {
            switch (Options.Version)
            {
                case FileDBDocumentVersion.Version1: TargetDocument = new FileDBDocument_V1(); break;
                case FileDBDocumentVersion.Version2: TargetDocument = new FileDBDocument_V2(); break;
            }
        }
        #endregion

        //serializes an object into a filedb document
        public IFileDBDocument WriteObjectStructureToFileDBDocument(object graph)
        {
            PropertyInfo[] properties = graph.GetType().GetProperties();
            TargetDocument.Roots = SerializePropertyCollection(properties, graph).ToList();

            var tmpdocument = TargetDocument;
            InitTargetDocument();
            return tmpdocument;
        }

        //Batch Serializing for a list of properties
        private IEnumerable<FileDBNode> SerializePropertyCollection(IEnumerable<PropertyInfo> properties, object parentObject)
        {
            foreach (var property in properties)
            {
                yield return BuildNode(property, parentObject);
            }
        }

        //parent switch for node types
        private FileDBNode BuildNode(PropertyInfo property, object parentObject)
        {
            Type PropertyType = property.PropertyType;

            //Note: IsPrimitive is the extension method, which does NOT match with the property IsPrimitive!!!!
            if (PropertyType.IsPrimitiveOrString())
            {
                //if primitive -> attrib, content to bytes, doneu
                return BuildSingleValueAttrib(parentObject, property);
            }
            //Arrays
            else if (PropertyType.IsArray())
            {
                return BuildArray(property, parentObject);
            }
            //simple Reference Types should be the only thing here
            else if(!PropertyType.IsEnumerable())
            {
                //if reference type -> build tag with child properties
                return BuildTagFromProperty(parentObject, property);
            }

            throw new InvalidOperationException($"PropertyType {PropertyType.Name} could not be resolved to a FileDB document element.");
        }

        private FileDBNode BuildArray(PropertyInfo property, object graph)
        {
            Type ArrayContentType = property.PropertyType.GetElementType();

            //primitive arrays
            if (ArrayContentType.IsPrimitiveType())
            {
                //if array -> attrib, array content to bytes, done
                return BuildPrimitiveArrayAttrib(graph, property);
            }
            //string arrays
            else if (ArrayContentType.IsStringType())
            {
                return BuildStringArray(graph, property);
            }
            //reference type array
            else
            {
                return BuildReferenceArray(graph, property);
            }
        }

        #region CONSTRUCT_ATTRIB
        //Single Values

        /// <summary>
        /// Constructs a FileDB Attrib in the target document from the property <paramref name="ObjectProperty"/> in the object instance <paramref name="graph"/> 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="ObjectProperty"></param>
        /// <returns>The constructed attribute</returns>
        private Attrib BuildSingleValueAttrib(object graph, PropertyInfo ObjectProperty)
        {
            var PrimitiveObjectInstance = ObjectProperty.GetValue(graph);
            Attrib attr = TargetDocument.AddAttrib(ObjectProperty.Name);
            return ConstructAttrib(PrimitiveObjectInstance, attr);
        }

        /// <summary>
        /// Constructs a FileDB Attrib in the target document out of the object instance <paramref name="PrimitiveObjectInstance"/>
        /// </summary>
        /// <param name="PrimitiveObjectInstance"></param>
        /// <returns>The constructed attribute. The Name of the returned attrib will be None.</returns>
        private Attrib BuildAnonymousSingleValueAttrib(object PrimitiveObjectInstance)
        {
            Attrib attr = TargetDocument.AddAttrib("None");
            return ConstructAttrib(PrimitiveObjectInstance, attr);
        }

        private Attrib ConstructAttrib(object PrimitiveObjectInstance, Attrib AttribInject)
        {
            if (PrimitiveObjectInstance.GetType().IsStringType())
            {
                AttribInject.Content = PrimitiveObjectInstance is EncodingAwareString ? ((EncodingAwareString)PrimitiveObjectInstance).GetBytes() : Options.DefaultEncoding.GetBytes((String)PrimitiveObjectInstance);
            }
            else if (PrimitiveObjectInstance.GetType().IsPrimitiveType())
            {
                AttribInject.Content = PrimitiveConverter.GetBytes(PrimitiveObjectInstance);
            }
            return AttribInject;
        }

        #endregion

        #region CONSTRUCT_TAG

        /// <summary>
        /// Constructs a FileDB Tag in the target document from the property <paramref name="ObjectProperty"/> in the object instance <paramref name="graph"/> 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="ObjectProperty"></param>
        /// <returns>The constructed Tag with all child Nodes</returns>
        private Tag BuildTagFromProperty(object graph, PropertyInfo ObjectProperty)
        {
            //Get the instance of the property for our specific object as well as the properties of its type.
            var ValueObjectInstance = ObjectProperty.GetValue(graph);
            Tag t = TargetDocument.AddTag(ObjectProperty.Name);
            return ConstructTag(ValueObjectInstance, t);
        }

        /// <summary>
        /// Constructs a FileDB Tag out of the object instance <paramref name="ValueObjectInstance"/>. <paramref name="ValueObjectInstance"/> cannot be a primitive or a string. For primitives, use <seealso cref="BuildAnonymousSingleValueAttrib"/>.
        /// </summary>
        /// <param name="ValueObjectInstance">Cannot be a primitive or a string. For primitives, use <seealso cref="BuildAnonymousSingleValueAttrib"/>.</param>
        /// <returns>the Tag, or an <seealso cref="InvalidOperationException"/> if <paramref name="ValueObjectInstance"/> is a primitive</returns>
        private Tag BuildTagFromAnonymousObject(object ValueObjectInstance)
        {
            if (ValueObjectInstance.GetType().IsPrimitiveOrString())
            {
                throw new InvalidOperationException("Anonymous FileDB Tags cannot be instantiated from primitive objects or strings.");
            }
            Tag t = TargetDocument.AddTag("None");
            return ConstructTag(ValueObjectInstance, t);
        }

        private Tag ConstructTag(object ValueObjectInstance, Tag TagInject)
        {
            PropertyInfo[] properties = ValueObjectInstance.GetType().GetProperties();
            foreach (var property in properties)
            {
                TagInject.AddChild(BuildNode(property, ValueObjectInstance));
            }
            return TagInject;
        }
        #endregion

        //Array of primitive Types
        private Attrib BuildPrimitiveArrayAttrib(object ArrayObjectInstance, PropertyInfo ObjectProperty)
        {
            Attrib attr = TargetDocument.AddAttrib(ObjectProperty.Name);
            Array arrayObject = (Array)ObjectProperty.GetValue(ArrayObjectInstance);
            attr.Content = BuildPrimitiveArrayContent(arrayObject);
            return attr;
        }

        //Array of Primitives - Content is a single stream of bytes
        private byte[] BuildPrimitiveArrayContent(Array ArrayObject)
        {
            //builds a byte array out of Array Content
            using (MemoryStream ContentStream = new MemoryStream())
            {
                for (int i = 0; i < ArrayObject.Length; i++)
                {
                    var SingleValue = ArrayObject.GetValue(i);
                    ContentStream.Write(PrimitiveConverter.GetBytes(SingleValue));
                }
                return ContentStream.ToArray();
            }
        }

        //Array of Reference Types - Array is a collection of none tags
        private Tag BuildReferenceArray(object ArrayObjectInstance, PropertyInfo ObjectProperty)
        {
            return BuildMultiNodeArray(ArrayObjectInstance, ObjectProperty, BuildTagFromAnonymousObject);
        }

        //Array of Strings - Array is a collection of none objects
        private Tag BuildStringArray(object ArrayObjectInstance, PropertyInfo ObjectProperty)
        {
            return BuildMultiNodeArray(ArrayObjectInstance, ObjectProperty, BuildAnonymousSingleValueAttrib);
        }

        private Tag BuildMultiNodeArray(object ArrayObjectInstance, PropertyInfo ObjectProperty, Func<object, FileDBNode> ChildCreationFunction)
        {
            Tag t = TargetDocument.AddTag(ObjectProperty.Name);
            Array ArrayObject = (Array)ObjectProperty.GetValue(ArrayObjectInstance);

            //loop through array objects and add them as anonymous attribs
            for (int i = 0; i < ArrayObject.Length; i++)
            {
                //get the array entry object instance, construct a tag and add it to the list!
                var SingleValueInArray = ArrayObject.GetValue(i);
                t.AddChild(ChildCreationFunction.Invoke(SingleValueInArray));
            }
            return t;
        }
    }
}
