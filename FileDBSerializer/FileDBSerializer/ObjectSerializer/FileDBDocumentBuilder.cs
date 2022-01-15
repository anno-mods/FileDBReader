using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.Serialization;
using System.Reflection;
using System.IO;

namespace FileDBSerializing.ObjectSerializer
{
    public enum FileDBDocumentVersion { Version1, Version2 }

    //allows to serialize a FileDBDocument directly from a class structure.

    public class FileDBDocumentBuilder<T> : IFormatter
    {
        public SerializationBinder Binder { get; set; }
        public StreamingContext Context { get; set; }
        public ISurrogateSelector SurrogateSelector { get; set; }

        private IFileDBDocument TargetDocument;
        private FileDBDocumentVersion _version;
        private Type _type;
        private PrimitiveTypeConverter PrimitiveConverter = new PrimitiveTypeConverter();

        public FileDBDocumentBuilder(FileDBDocumentVersion version)
        {
            _type = typeof(T);
            _version = version;
            InitTargetDocument();
        }

        private void InitTargetDocument()
        {
            switch (_version)
            {
                case FileDBDocumentVersion.Version1: TargetDocument = new FileDBDocument_V1(); break;
                case FileDBDocumentVersion.Version2: TargetDocument = new FileDBDocument_V2(); break;
            }
        }

        //serializes an object into a filedb document
        public void Serialize(Stream serializationStream, object graph)
        {
            PropertyInfo[] properties = _type.GetProperties();
            TargetDocument.Roots = SerializePropertyCollection(properties, graph).ToList();

            FileDBSerializer GenericSerializer = new FileDBSerializer();
            GenericSerializer.Serialize(TargetDocument, serializationStream);

            InitTargetDocument();
        }

        //serializes from a filedb document into an object
        public object Deserialize(Stream serializationStream)
        {
            Console.WriteLine("I am too lazy to write this right now. To be added!");
            throw new NotImplementedException();
        }

        private IEnumerable<FileDBNode> SerializePropertyCollection(IEnumerable<PropertyInfo> properties, object parentObject)
        {
            foreach (var property in properties)
            {
                yield return BuildNode(property, parentObject);
            }
        }

        private FileDBNode BuildNode(PropertyInfo property, object parentObject)
        {
            Type PropertyType = property.PropertyType;
            //Note: IsPrimitive is the extension method, which does NOT match with the property!!!!
            if (PropertyType.IsPrimitiveType())
            {
                //if primitive -> attrib, content to bytes, done
                return BuildAttrib(parentObject, property);
            }
            else if (PropertyType.IsArray())
            {
                //if array -> attrib, array content to bytes, done
                return BuildArrayAttrib(parentObject, property);
            }
            else 
            {
                //if reference type -> build tag with child properties
                return BuildTag(parentObject, property);
            }

            throw new Exception("PropertyType could not be resolved to a FileDB document element");
        }

        private Attrib BuildAttrib(object ObjectInstance, PropertyInfo ObjectProperty)
        {
            var content = ObjectProperty.GetValue(ObjectInstance);
            Attrib attr = TargetDocument.AddAttrib(ObjectProperty.Name);
            attr.Content = BuildSingleValueContent(content);
            return attr;
        }

        private Attrib BuildArrayAttrib(object ObjectInstance, PropertyInfo ObjectProperty)
        {
            Attrib attr = TargetDocument.AddAttrib(ObjectProperty.Name);
            Array arrayObject = (Array)ObjectProperty.GetValue(ObjectInstance);
            attr.Content = BuildArrayContent(arrayObject);
            return attr;
        }

        private byte[] BuildSingleValueContent(object obj)
        {
            return PrimitiveConverter.GetBytes(obj);
        }

        private byte[] BuildArrayContent(Array ArrayObject)
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

        private Tag BuildTag(object ObjectInstance, PropertyInfo ObjectProperty)
        {
            //Get the instance of the property for our specific object as well as the properties of its type.
            var ValueObjectInstance = ObjectProperty.GetValue(ObjectInstance);
            PropertyInfo[] properties = ValueObjectInstance.GetType().GetProperties();

            //init and return the tag.
            Tag tag = TargetDocument.AddTag(ObjectProperty.Name);
            foreach (var property in properties)
            {
                tag.AddChild(BuildNode(property, ValueObjectInstance));
            }
            return tag;
        }
    }
}
