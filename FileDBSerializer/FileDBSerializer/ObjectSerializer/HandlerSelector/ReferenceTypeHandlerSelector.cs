using FileDBSerializer.ObjectSerializer.SerializationHandlers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.HandlerSelector
{
    public class ReferenceTypeHandlerSelector : IHandlerSelector
    {
        public HandlerType GetHandlerFor(Type itemType, IEnumerable<Attribute> customAttributes)
        {
            //make check for ITuple here laterz(tm)
            return HandlerType.Reference;
        }
    }
}
