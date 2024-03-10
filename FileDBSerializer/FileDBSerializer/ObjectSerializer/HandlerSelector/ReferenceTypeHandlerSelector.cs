using System;
using System.Collections.Generic;

namespace AnnoMods.BBDom.ObjectSerializer.HandlerSelector
{
    public class ReferenceTypeHandlerSelector : IHandlerSelector
    {
        public HandlerType GetHandlerFor(Type itemType, IEnumerable<Attribute> customAttributes)
        {
            return HandlerType.Reference;
        }
    }
}
