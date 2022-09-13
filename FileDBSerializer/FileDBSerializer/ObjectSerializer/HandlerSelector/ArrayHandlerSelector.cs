﻿using FileDBSerializer.ObjectSerializer.SerializationHandlers;
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


            if (customAttributes.Any((attr) => attr.GetType() == typeof(FlatArrayAttribute)))
            {
                return HandlerType.FlatArray;
            }
            else if (arrayContentType.IsPrimitiveType())
            {
                return HandlerType.PrimitiveArray;
            }
            else if (arrayContentType.IsStringType())
            {
                return HandlerType.StringArray;
            }
            else if (arrayContentType.IsReference())
            {
                return HandlerType.ReferenceArray;
            }

            //fuck the implementation for now
            throw new NotImplementedException();
        }
    }
}
