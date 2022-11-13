using FileDBSerializer.ObjectSerializer.SerializationHandlers;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.HandlerSelector
{
    public class ListHandlerSelector
    {
        public HandlerType GetHandlerFor(Type itemType, IEnumerable<Attribute> customAttributes)
        {
            if (!itemType.IsList())
                throw new InvalidProgramException("ListHandlerSelector expects a list type");
            Type listContentType = itemType.GetNullableType().GetGenericArguments().Single();

            if (customAttributes.ContainsAttribute<FlatArrayAttribute>())
            {
                return HandlerType.FlatArray;
            }

            return HandlerType.List;

            //fuck the implementation for now
            throw new NotImplementedException($"No handler found for {itemType}");
        }
    }
}
