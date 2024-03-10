using System.Collections.Generic;

namespace AnnoMods.BBDom.ObjectSerializer.SerializationHandlers
{
    public interface ISerializationHandler
    {
        public IEnumerable<BBNode> Handle(object? item, string tagName, IBBDocument workingDocument, BBSerializerOptions options);
    }
}
