using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializer
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
    }
}
