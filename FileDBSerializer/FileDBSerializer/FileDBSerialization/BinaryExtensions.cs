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
        #region BinaryReaderExtensions
        internal static String ReadString0(this BinaryReader reader)
        {
            StringBuilder sb = new StringBuilder();
            char c;
            while ((c = reader.ReadChar()) != 0)
            {
                sb.Append(c);
            }
            return sb.ToString();
        }
        internal static long Position(this BinaryReader reader)
        {
            return reader.BaseStream.Position;
        }

        internal static void SetPosition(this BinaryReader reader, long Position)
        {
            reader.BaseStream.Position = Position; 
        }

        #endregion

        #region BinaryWriterExtensions

        /// <summary>
        /// Writes a zero-terminated ascii string
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="AsciiString"></param>
        /// <returns>The amount of bytes written</returns>
        internal static int WriteString0(this BinaryWriter writer, String AsciiString)
        {
            var bytes = new ASCIIEncoding().GetBytes(AsciiString);
            writer.Write(bytes);
            writer.Write((byte)0);
            return bytes.Length + 1; 
        }

        internal static long Position(this BinaryWriter writer)
        {
            return writer.BaseStream.Position;
        }

        internal static void SetPosition(this BinaryWriter writer, long Position)
        {
            writer.BaseStream.Position = Position;
        }

        #endregion
    }
}
