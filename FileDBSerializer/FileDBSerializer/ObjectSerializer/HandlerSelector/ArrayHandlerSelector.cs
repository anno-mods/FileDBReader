using System;
using System.Collections.Generic;

namespace AnnoMods.BBDom.ObjectSerializer.HandlerSelector
{
    public class ArrayHandlerSelector : IHandlerSelector
    {
        public HandlerType GetHandlerFor(Type itemType, IEnumerable<Attribute> customAttributes)
        {            
            Type arrayContentType = itemType.GetNullableType().GetElementType()!;

            if (arrayContentType.IsPrimitiveType())
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
