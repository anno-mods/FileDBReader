using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
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
