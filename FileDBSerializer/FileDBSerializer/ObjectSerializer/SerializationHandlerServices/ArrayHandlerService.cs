using FileDBSerializer.ObjectSerializer.SerializationHandlers;
using FileDBSerializing.ObjectSerializer;
using System;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlerServices
{
    public class ArrayHandlerService : IHandlerService
    {
        public ISerializationHandler GetHandlerFor(Type propertyType)
        {
            Type arrayContentType = propertyType.GetElementType()!;

            if (arrayContentType.IsPrimitiveType())
            {
                return new PrimitiveArrayHandler();
            }
            else if (arrayContentType.IsStringType() || arrayContentType.IsPrimitiveListType())
            { 
            
            }
            //else -> reference type array!
            //...

            //fuck the implementation for now
            throw new NotImplementedException();
        }
    }
}
