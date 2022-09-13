using FileDBSerializer.ObjectSerializer.HandlerSelector;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public class HandlerProvider
    {
        private static Dictionary<HandlerType, ISerializationHandler> handlers = new()
        {
            { HandlerType.String, new StringHandler() },
            { HandlerType.Primitive, new PrimitiveHandler() },
            { HandlerType.Reference, new ReferenceTypeHandler() },
            { HandlerType.ReferenceArray, new ReferenceArrayHandler() },
            { HandlerType.PrimitiveArray, new PrimitiveArrayHandler() },
            { HandlerType.ITuple, new TupleHandler() },
            { HandlerType.FlatArray, new FlatArrayHandler() },
            { HandlerType.StringArray, new StringArrayHandler()}
            
        };

        public static ISerializationHandler GetFromType(HandlerType handlerType)
        {
            if (handlers.TryGetValue(handlerType, out var val)) 
            {
                return val;
            }
            throw new NotImplementedException($"No handler implemented for {handlerType}");
        }

        public static ISerializationHandler GetHandlerFor(PropertyInfo property)
        {
            return GetHandlerFor(property.PropertyType, property.GetCustomAttributes());
        }

        public static ISerializationHandler GetHandlerFor(Type itemType, IEnumerable<Attribute> customAttributes)
        {
            var type = Selectors.TopLevelHandlerService.GetHandlerFor(itemType, customAttributes);
            return GetFromType(type);
        }
    }
}
