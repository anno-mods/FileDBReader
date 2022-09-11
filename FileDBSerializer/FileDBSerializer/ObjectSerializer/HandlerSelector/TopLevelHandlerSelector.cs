using FileDBSerializer.ObjectSerializer.SerializationHandlers;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.HandlerSelector
{
    public class TopLevelHandlerSelector
    {
        public HandlerType GetHandlerFor(PropertyInfo propertyInfo)
        {
            var propertyType = propertyInfo.GetNullablePropertyType();
            if (propertyType.IsStringType())
                return HandlerType.String;

            if (propertyType.IsPrimitiveType())
                return HandlerType.Primitive;

            if (propertyType.IsArray())
                return HandlerSelection.ArrayHandlerService.GetHandlerFor(propertyInfo);

            //this needs to be after array
            if (propertyType.IsReference())
                return HandlerSelection.ReferenceTypeHandlerService.GetHandlerFor(propertyInfo);

            throw new InvalidOperationException($"PropertyType {propertyType.Name} could not be resolved to a FileDB document element.");

        }
    }
}
