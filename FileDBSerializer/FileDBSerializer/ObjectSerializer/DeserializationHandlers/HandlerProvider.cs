using AnnoMods.BBDom.ObjectSerializer.HandlerSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AnnoMods.BBDom.ObjectSerializer.DeserializationHandlers
{
    public class HandlerProvider
    {
        private static Dictionary<HandlerType, IDeserializationHandler> handlers = new()
        {
            { HandlerType.Primitive, new PrimitiveHandler() },
            { HandlerType.Reference, new ReferenceTypeHandler() },
            { HandlerType.PrimitiveArray, new PrimitiveArrayHandler() },
            { HandlerType.ReferenceArray, new ReferenceArrayHandler() },
            { HandlerType.String, new StringHandler()},
            { HandlerType.ITuple, new TupleHandler() },
            { HandlerType.FlatArray, new FlatArrayHandler() },
            { HandlerType.List, new ListHandler() }
        };

        public static IDeserializationHandler GetFromType(HandlerType handlerType)
        {
            if (handlers.TryGetValue(handlerType, out var val))
            {
                return val;
            }
            throw new NotImplementedException($"No handler implemented for {handlerType}");
        }

        public static IDeserializationHandler GetHandlerFor(PropertyInfo property)
        {
            return GetHandlerFor(property.PropertyType, property.GetCustomAttributes());
        }

        public static IDeserializationHandler GetHandlerFor(Type itemType, IEnumerable<Attribute> customAttributes)
        {
            var type = Selectors.TopLevelHandlerService.GetHandlerFor(itemType, customAttributes);
            return GetFromType(type);
        }

        internal static object GetHandlerFor(object propertyInfo)
        {
            throw new NotImplementedException();
        }

        public static IDeserializationHandler GetHandlerFor(Type itemType)
        {
            return GetHandlerFor(itemType, Enumerable.Empty<Attribute>());
        }
    }
}
