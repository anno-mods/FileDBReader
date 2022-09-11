using FileDBSerializer.ObjectSerializer;
using FileDBSerializer.ObjectSerializer.SerializationHandlers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FileDBSerializing.ObjectSerializer
{
    public class FileDBDocumentSerializer 
    {
        private FileDBSerializerOptions Options;
        private IFileDBDocument TargetDocument;

        public FileDBDocumentSerializer(FileDBSerializerOptions options)
        {
            Options = options;
            InitTargetDocument();
        }

        private void InitTargetDocument()
        {
            TargetDocument = DependencyVersions.GetDocument(Options.Version);
        }

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

        private IEnumerable<FileDBNode> BuildNode(PropertyInfo property, object parentObject)
        {
            var handler = HandlerProvider.GetHandlerFor(property);
            var node = handler.Handle(parentObject, property, TargetDocument, Options);
            return node.AsEnumerable();
        }
    }
}
