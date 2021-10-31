using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    static class BinaryExtensions
    {
        public static String ReadString0(this BinaryReader reader)
        {
            StringBuilder sb = new StringBuilder();
            char c;
            while ((c = reader.ReadChar()) != 0)
            {
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static void WriteString0(this BinaryWriter writer, String AsciiString)
        {
            Encoding e = new ASCIIEncoding();
            writer.Write(e.GetBytes(AsciiString));
            writer.Write((byte)0);
        }
    }
}
