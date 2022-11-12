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
            IEnumerable<PropertyInfo> properties = graph.GetType().GetPropertiesWithOrder();
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
                var handler = HandlerProvider.GetHandlerFor(property);

                var item = property.GetValue(parentObject);
                string tagName = property.GetNameWithRenaming();

                foreach (var _ in handler.Handle(item, tagName, TargetDocument, Options))
                {
                    yield return _;
                }
            }
        }
    }
}
