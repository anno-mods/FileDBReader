using FileDBSerializer.ObjectSerializer.SerializationHandlers;
using System;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlerServices
{
    public interface IHandlerService
    {
        public ISerializationHandler GetHandlerFor(Type propertyType);
    }
}
