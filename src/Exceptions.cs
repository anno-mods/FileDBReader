using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBReader.src
{
    class InvalidConversionException : Exception
    {
        public Type TargetType;
        public String NodeName;
        public String ContentToConvert;
        public InvalidConversionException(Type Target, String Node, String Content) 
        {
            ContentToConvert = Content;
            NodeName = Node;
            TargetType = Target; 
        }

        public InvalidConversionException(string message, Type Target, String Node, String Content)
        : base(message)
        {
            ContentToConvert = Content;
            NodeName = Node;
            TargetType = Target;
        }

        public InvalidConversionException(string message, Exception inner, Type Target, String Node, String Content)
            : base(message, inner)
        {
            ContentToConvert = Content;
            NodeName = Node;
            TargetType = Target;
        }
    }
}
