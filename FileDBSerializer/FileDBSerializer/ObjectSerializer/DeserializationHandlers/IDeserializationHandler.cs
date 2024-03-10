using System;
using System.Collections.Generic;

namespace AnnoMods.BBDom.ObjectSerializer.DeserializationHandlers
{
    public interface IDeserializationHandler
    {
        public object? Handle(IEnumerable<BBNode> nodes, Type targetType, BBSerializerOptions options);
    }
}
