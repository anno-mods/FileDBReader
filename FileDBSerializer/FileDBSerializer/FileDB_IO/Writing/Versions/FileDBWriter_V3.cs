using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    internal class FileDBWriter_V3 : FileDBWriter_V2
    {
        public FileDBWriter_V3(Stream s) : base(s)
        { 
        
        }
        new public void WriteMagicBytes()
        {
            Writer!.Write(Versioning.GetMagicBytes(FileDBDocumentVersion.Version3));
        }
    }
}
