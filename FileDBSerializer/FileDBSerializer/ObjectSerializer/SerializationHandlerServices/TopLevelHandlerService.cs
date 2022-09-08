using FileDBSerializer.ObjectSerializer.SerializationHandlers;
using FileDBSerializing.ObjectSerializer;
using System;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlerServices
{
    public class TopLevelHandlerService
    {
        public ISerializationHandler GetHandlerFor(Type propertyType)
        {
            //Note: IsPrimitive is the extension method, which does NOT match with the property IsPrimitive!!!!
            if (propertyType.IsPrimitiveOrString())
                return HandlerServices.PrimitiveOrStringHandlerService.GetHandlerFor(propertyType);

            if (propertyType.IsArray())
                return HandlerServices.ArrayHandlerService.GetHandlerFor(propertyType);

            if (propertyType.IsEnumerable())
                return new TagHandler();

            throw new InvalidOperationException($"PropertyType {propertyType.Name} could not be resolved to a FileDB document element.");

        }
    }
}
