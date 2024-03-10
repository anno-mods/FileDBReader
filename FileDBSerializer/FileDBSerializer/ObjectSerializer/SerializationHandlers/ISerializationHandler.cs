using AnnoMods.BBDom;
using AnnoMods.ObjectSerializer;
using System.Collections.Generic;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public interface ISerializationHandler
    {
        public IEnumerable<BBNode> Handle(object? item, string tagName, IBBDocument workingDocument, BBSerializerOptions options);
    }
}
