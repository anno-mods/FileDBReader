using FileDBSerializer.ObjectSerializer.SerializationHandlers;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.HandlerSelector
{
    public class ArrayHandlerSelector : IHandlerSelector
    {
        public HandlerType GetHandlerFor(Type itemType, IEnumerable<Attribute> customAttributes)
        {            
            Type arrayContentType = itemType.GetNullableType().GetElementType()!;

            if (customAttributes.ContainsAttribute<FlatArrayAttribute>())
            {
                return HandlerType.FlatArray;
            }
            else if (arrayContentType.IsPrimitiveType())
            {
                return HandlerType.PrimitiveArray;
            }
            else if (
                arrayContentType.IsReference()
                || arrayContentType.IsStringType()
                || arrayContentType.IsTuple())
            {
                return HandlerType.ReferenceArray;
            }

            //fuck the implementation for now
            throw new NotImplementedException($"No handler found for {itemType}");
        }
    }
}
