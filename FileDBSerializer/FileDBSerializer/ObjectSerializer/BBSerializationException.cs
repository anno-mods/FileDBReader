using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoMods.BBDom.ObjectSerializer
{
    public class BBSerializationException : Exception
    {
        public BBSerializationException() : base()
        {
            
        }

        public BBSerializationException(String message) : base(message)
        { 
        
        }

        public BBSerializationException(String Message, Exception InnerException) : base(Message, InnerException)
        { 
        
        }
    }
}
