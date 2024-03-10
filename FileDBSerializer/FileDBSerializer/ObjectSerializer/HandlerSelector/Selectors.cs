﻿using AnnoMods.BBDom.ObjectSerializer.HandlerSelector;

namespace AnnoMods.BBDom.ObjectSerializer
{
    public class Selectors
    {
        public static TopLevelHandlerSelector TopLevelHandlerService = new TopLevelHandlerSelector();
        public static ArrayHandlerSelector ArrayHandlerService = new ArrayHandlerSelector();
        public static ReferenceTypeHandlerSelector ReferenceTypeHandlerService = new ReferenceTypeHandlerSelector();
        public static ListHandlerSelector ListHandlerService = new ListHandlerSelector();
    }
}
