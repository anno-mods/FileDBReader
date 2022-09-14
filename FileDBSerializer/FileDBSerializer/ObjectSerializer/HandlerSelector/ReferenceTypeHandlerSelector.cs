using FileDBSerializer.ObjectSerializer.SerializationHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FileDBSerializer.ObjectSerializer.HandlerSelector
{
    public class ReferenceTypeHandlerSelector : IHandlerSelector
    {
        public HandlerType GetHandlerFor(Type itemType, IEnumerable<Attribute> customAttributes)
        {
            return HandlerType.Reference;
        }
    }
}
