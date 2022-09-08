using FileDBSerializer.ObjectSerializer.SerializationHandlers;
using FileDBSerializing.ObjectSerializer;
using System;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlerServices
{
    public class PrimitiveOrStringHandlerService : IHandlerService
    {
        public ISerializationHandler GetHandlerFor(Type propertyType)
        {
            if (propertyType.IsStringType()) 
                return new StringSingleValueHandler();

            if (propertyType.IsPrimitiveType())
                return new PrimitiveSingleValueHandler();

            throw new InvalidOperationException($"PropertyType {propertyType.Name} could not be resolved to a FileDB document element.");
        }
    }
}
