using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing.ObjectSerializer
{
    public class FileDBSerializationException : Exception
    {
        public FileDBSerializationException() : base()
        {
            
        }

        public FileDBSerializationException(String message) : base(message)
        { 
        
        }

        public FileDBSerializationException(String Message, Exception InnerException) : base(Message, InnerException)
        { 
        
        }
    }
}
