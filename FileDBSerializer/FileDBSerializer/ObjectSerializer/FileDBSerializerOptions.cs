using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing.ObjectSerializer
{
    public class FileDBSerializerOptions
    {
        public FileDBDocumentVersion Version;

        public FileDBSerializerOptions()
        {
            Version = FileDBDocumentVersion.Version1;
        }
    }
}
