using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 

namespace FileDBSerializing
{
    public static class VersionDetector
    {
        public static int GetCompressionVersion(Stream fs)
        {
            long Position = fs.Position;
            int magicByteSize = FileDBDocument_V2._magic_byte_count;
            //check for version 2
            fs.Position = fs.Length - magicByteSize;
            byte[] magics = new byte[magicByteSize];
            fs.Read(magics, 0, magicByteSize);
            fs.Position = Position;
            if (magics.SequenceEqual<byte>(FileDBDocument_V2._magic_bytes))
                return 2;
            else
                return 1;
        }
    }
}
