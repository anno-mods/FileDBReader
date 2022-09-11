using FileDBSerializer.ObjectSerializer.HandlerSelector;

namespace FileDBSerializer.ObjectSerializer
{
    public class HandlerSelection
    {
        public static TopLevelHandlerSelector TopLevelHandlerService = new TopLevelHandlerSelector();
        public static ArrayHandlerSelector ArrayHandlerService = new ArrayHandlerSelector();
        public static ReferenceTypeHandlerSelector ReferenceTypeHandlerService = new ReferenceTypeHandlerSelector();
    }
}
