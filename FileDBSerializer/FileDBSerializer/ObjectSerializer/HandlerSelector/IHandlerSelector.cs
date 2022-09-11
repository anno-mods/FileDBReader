using FileDBSerializer.ObjectSerializer.SerializationHandlers;
using System;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.HandlerSelector
{
    public interface IHandlerSelector
    {
        public HandlerType GetHandlerFor(PropertyInfo propertyInfo);
    }
}
