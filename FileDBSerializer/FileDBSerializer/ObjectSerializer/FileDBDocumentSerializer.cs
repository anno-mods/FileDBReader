using FileDBSerializing.EncodingAwareStrings;
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
                foreach (var _ in BuildNode(property, parentObject))
                {
                    yield return _;
                }
            }
        }

        //parent switch for node types
        private IEnumerable<FileDBNode> BuildNode(PropertyInfo property, object parentObject)
        {
            Type PropertyType = property.GetNullablePropertyType();

            //skip if ignore default
            object? property_instance = property.GetValue(parentObject);
            if (property_instance is not null && property_instance.Equals(PropertyType.GetDefault()) && Options.SkipDefaultedValues) return Enumerable.Empty<FileDBNode>();

            //Note: IsPrimitive is the extension method, which does NOT match with the property IsPrimitive!!!!
            if (PropertyType.IsPrimitiveOrString())
            {
                if (property_instance is null && Options.SkipSimpleNullValues)
                    return Enumerable.Empty<FileDBNode>();

                //if primitive -> attrib, content to bytes, doneu
                return new FileDBNode[] { BuildSingleValueAttrib(parentObject, property) };
            }
            //Arrays
            else if (PropertyType.IsArray())
            {
                if (property_instance is null && Options.SkipSimpleNullValues && PropertyType.IsPrimitiveArray())
                    return Enumerable.Empty<FileDBNode>();

                return BuildArray(property, parentObject);
            }
            //simple Reference Types should be the only thing here
            else if(!PropertyType.IsEnumerable())
            {
                //if reference type -> build tag with child properties
                return new FileDBNode[] { BuildTagFromProperty(parentObject, property) };
            }

            //we should not get here.
            throw new InvalidOperationException($"PropertyType {PropertyType.Name} could not be resolved to a FileDB document element.");
        }

        private IEnumerable<FileDBNode> BuildArray(PropertyInfo property, object graph)
        {
            Type ArrayContentType = property.GetNullablePropertyType().GetElementType()!;

            // primitive arrays
            if (ArrayContentType.IsPrimitiveType())
            {
                //if array -> attrib, array content to bytes, done
                yield return BuildPrimitiveArrayAttrib(graph, property);
            }
            // string arrays
            else if (ArrayContentType.IsStringType())
            {
                bool isFlat = property.HasAttribute<FlatArrayAttribute>();
                if (isFlat)
                {
                    IEnumerable<Attrib> arrayContainer = BuildStringArray(graph, property);
                    foreach (FileDBNode node in arrayContainer)
                        yield return node;
                }
                else
                    yield return BuildStringArrayTag(graph, property);
            }
            else if (ArrayContentType.IsPrimitiveArray())
            {
                // TODO
                throw new NotImplementedException();
            }
            // reference type array
            else
            {
                Tag arrayContainer = BuildReferenceArrayTag(graph, property);
                bool isFlat = property.HasAttribute<FlatArrayAttribute>();
                if (isFlat)
                {
                    foreach (FileDBNode child in arrayContainer.Children)
                    {
                        child.ID = arrayContainer.ID;
                        child.Parent = arrayContainer.Parent;
                        yield return child;
                    }
                }
                else
                    yield return arrayContainer;
                
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

            if (PrimitiveObjectInstance is null)
            {
                attr.Content = new byte[0];
                return attr;
            } 
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

            if (ValueObjectInstance is null) return t;

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
                TagInject.AddChildren(BuildNode(property, ValueObjectInstance));
            }
            return TagInject;
        }
        #endregion

        //Array of primitive Types
        private Attrib BuildPrimitiveArrayAttrib(object ArrayObjectInstance, PropertyInfo ObjectProperty)
        {
            Attrib attr = TargetDocument.AddAttrib(ObjectProperty.Name);
            Array? arrayObject = ObjectProperty.GetValue(ArrayObjectInstance) as Array;

            attr.Content = arrayObject is not null ? BuildPrimitiveArrayContent(arrayObject) : new byte[0];
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

        // Array of Reference Types - Array is a collection of none tags
        private Tag BuildReferenceArrayTag(object arrayObject, PropertyInfo objectProperty)
        {
            return BuildMultiNodeArray(arrayObject, objectProperty, BuildTagFromAnonymousObject);
        }

        private IEnumerable<Attrib> BuildStringArray(object arrayObject, PropertyInfo objectProperty)
        {
            return BuildArrayElements(arrayObject, objectProperty, 
                (x) => ConstructAttrib(x, TargetDocument.AddAttrib(objectProperty.Name)));
        }

        private Tag BuildStringArrayTag(object arrayObject, PropertyInfo objectProperty)
        {
            Tag tag = TargetDocument.AddTag(objectProperty.Name);
            tag.AddChildren(BuildArrayElements(arrayObject, objectProperty,
                (x) => ConstructAttrib(x, TargetDocument.AddAttrib("None"))));
            return tag;
        }

        private Tag BuildMultiNodeArray(object arrayObject, PropertyInfo objectProperty, Func<object, FileDBNode> elementConstructor)
        {
            Tag tag = TargetDocument.AddTag(objectProperty.Name);
            tag.AddChildren(BuildArrayElements(arrayObject, objectProperty, elementConstructor));
            return tag;
        }

        private static IEnumerable<T> BuildArrayElements<T>(object arrayObject, PropertyInfo objectProperty, Func<object, T> elementConstructor)
        {
            Array? array = (Array?)objectProperty.GetValue(arrayObject);
            if (array is null)
                yield break;

            foreach (var element in array)
                yield return elementConstructor.Invoke(element);
        }
    }
}
