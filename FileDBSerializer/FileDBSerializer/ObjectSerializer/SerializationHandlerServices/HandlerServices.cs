using FileDBSerializer.ObjectSerializer.SerializationHandlerServices;

namespace FileDBSerializer.ObjectSerializer
{
    public class HandlerServices
    {
        public static TopLevelHandlerService TopLevelHandlerService = new TopLevelHandlerService();
        public static PrimitiveOrStringHandlerService PrimitiveOrStringHandlerService = new PrimitiveOrStringHandlerService();
        public static ArrayHandlerService ArrayHandlerService = new ArrayHandlerService();
    }
}
