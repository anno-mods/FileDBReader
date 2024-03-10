using System;
using System.Collections.Generic;

namespace AnnoMods.BBDom.ObjectSerializer.HandlerSelector
{
    public interface IHandlerSelector
    {
        public HandlerType GetHandlerFor(Type itemType, IEnumerable<Attribute> customAttributes);
    }
}
