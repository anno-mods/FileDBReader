using FileDBSerializer.ObjectSerializer.SerializationHandlers;
using System;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.HandlerSelector
{
    public class ReferenceTypeHandlerSelector : IHandlerSelector
    {
        public HandlerType GetHandlerFor(PropertyInfo propertyInfo)
        {
            //make check for IValue here laterz(tm)
            return HandlerType.Reference;
        }
    }
}
