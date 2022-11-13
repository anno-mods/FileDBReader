using FileDBSerializer.ObjectSerializer.SerializationHandlers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.HandlerSelector
{
    public interface IHandlerSelector
    {
        public HandlerType GetHandlerFor(Type itemType, IEnumerable<Attribute> customAttributes);
    }
}
