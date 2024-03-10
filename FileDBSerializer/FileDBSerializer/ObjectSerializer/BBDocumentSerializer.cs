using AnnoMods.BBDom;
using AnnoMods.BBDom.IO;
using AnnoMods.BBDom.ObjectSerializer.SerializationHandlers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AnnoMods.BBDom.ObjectSerializer
{
    public class BBDocumentSerializer 
    {
        private BBSerializerOptions Options;
        private BBDocument TargetDocument;

        public BBDocumentSerializer(BBSerializerOptions options)
        {
            Options = options;
            InitTargetDocument();
        }

        private void InitTargetDocument()
        {
            TargetDocument = new BBDocument();
        }

        //serializes an object into a filedb document
        public BBDocument WriteObjectStructureToBBDocument(object graph)
        {
            IEnumerable<PropertyInfo> properties = graph.GetType().GetPropertiesWithOrder();
            TargetDocument.Roots = SerializePropertyCollection(properties, graph).ToList();

            var tmpdocument = TargetDocument;
            InitTargetDocument();
            return tmpdocument;
        }

        //Batch Serializing for a list of properties
        private IEnumerable<BBNode> SerializePropertyCollection(IEnumerable<PropertyInfo> properties, object parentObject)
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
