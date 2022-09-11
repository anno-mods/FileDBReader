using FileDBSerializer.ObjectSerializer.HandlerSelector;

namespace FileDBSerializer.ObjectSerializer
{
    public class Selectors
    {
        public static TopLevelHandlerSelector TopLevelHandlerService = new TopLevelHandlerSelector();
        public static ArrayHandlerSelector ArrayHandlerService = new ArrayHandlerSelector();
        public static ReferenceTypeHandlerSelector ReferenceTypeHandlerService = new ReferenceTypeHandlerSelector();
    }
}
