using System;

namespace AnnoMods.BBDom.IO
{
    public class InvalidFileDBException : Exception
    {
        public InvalidFileDBException()
        { }

        public InvalidFileDBException(string message)
        : base(message)
        { }

        public InvalidFileDBException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
