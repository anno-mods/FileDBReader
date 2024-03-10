using System;

namespace AnnoMods.BBDom.IO
{
    public class InvalidBBException : Exception
    {
        public InvalidBBException()
        { }

        public InvalidBBException(string message)
        : base(message)
        { }

        public InvalidBBException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
