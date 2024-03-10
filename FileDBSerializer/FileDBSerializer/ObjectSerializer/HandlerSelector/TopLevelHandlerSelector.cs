using System;
using System.Collections.Generic;

namespace AnnoMods.BBDom.ObjectSerializer.HandlerSelector
{
    public class TopLevelHandlerSelector : IHandlerSelector
    {
        public HandlerType GetHandlerFor(Type itemType, IEnumerable<Attribute> customAttributes)
        {
            var propertyType = itemType.GetNullableType();
            if (propertyType.IsStringType())
                return HandlerType.String;

            if (propertyType.IsPrimitiveType())
                return HandlerType.Primitive;

            if (propertyType.IsArray())
                return Selectors.ArrayHandlerService.GetHandlerFor(itemType, customAttributes);

            if (propertyType.IsList())
                return Selectors.ListHandlerService.GetHandlerFor(itemType, customAttributes);

            if (propertyType.IsTuple())
                return HandlerType.ITuple;

            //this needs to be after array
            if (propertyType.IsReference())
                return Selectors.ReferenceTypeHandlerService.GetHandlerFor(itemType, customAttributes);

            throw new InvalidOperationException($"PropertyType {propertyType.Name} could not be resolved to a BB document element.");

        }
    }
}
