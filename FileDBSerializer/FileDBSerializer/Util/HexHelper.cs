using System;
using System.IO;
using System.Linq;

namespace AnnoMods.BBDom.Util
{

    //todo CLEAN THIS UP FFS

    /// <summary>
    /// Static Library of Methods that will help with BinHex, since System.Xml binhex has awful performance
    /// </summary>
    public static class HexHelper
    {
        public static string Flip(string BinHex)
        {
            string s = "";
            for (int i = BinHex.Length; i > 1; i -= 2)
            {
                s += BinHex.Substring(i - 2, 2);
            }
            return s;
        }

        public static byte[] ToBytes(Stream s)
        {
            long lastPos = s.Position;
            using (MemoryStream ms = new MemoryStream())
            {
                s.Position = 0;
                s.CopyTo(ms);
                s.Position = lastPos;
                return ms.ToArray();
            }
        }

        public static byte[] ToBytes(string BinHex)
        {
            return Enumerable.Range(0, BinHex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(BinHex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ToBinHex(Stream s)
        {
            var bytes = ToBytes(s);
            return ToBinHex(bytes);
        }

        public static string ToBinHex(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }
    }
}
