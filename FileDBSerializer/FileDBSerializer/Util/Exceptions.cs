using System;


namespace AnnoMods.BBDom.Util
{
    public class InvalidXmlDocumentInputException : Exception
    {
        public InvalidXmlDocumentInputException()
        { }

        public InvalidXmlDocumentInputException(string message)
        : base(message)
        { }

        public InvalidXmlDocumentInputException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
