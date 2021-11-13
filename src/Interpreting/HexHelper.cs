using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;

namespace FileDBReader
{

    //todo CLEAN THIS UP FFS

    /// <summary>
    /// Static Library of Methods that will help with BinHex, since System.Xml binhex has awful performance
    /// </summary>
    public static class HexHelper
    {
        public static String Flip(String BinHex)
        {
            String s = "";
            for (int i = BinHex.Length; i > 1; i -= 2)
            {
                s += BinHex.Substring(i - 2, 2);
            }
            return s;
        }

        public static byte[] ToBytes(Stream s)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                s.Position = 0;
                s.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static String ToBinHex(Stream s)
        {
            var bytes = ToBytes(s);
            return ToBinHex(bytes);
        }

        public static String ToBinHex(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static byte[] BytesFromBinHex(string BinHex)
        {
            return Enumerable.Range(0, BinHex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(BinHex.Substring(x, 2), 16))
                             .ToArray();
        }

        //using String.Join for performance optimization over for loops.
        public static String Join<T>(String BinaryData) where T : struct
        {
            var span = HexHelper.toSpan<T>(BinaryData).ToArray();
            return String.Join<T>(" ", span);
        }

        //Converts a Hex String to a span of bytes
        public static Span<T> toSpan<T>(String BinHex) where T : struct
        {
            int size = Marshal.SizeOf(default(T)) * 2;
            var bytes = new T[BinHex.Length / size];
            Type t = typeof(T);
            for (var i = 0; i < bytes.Length; i++)
            {
                try
                {
                    bytes[i] = (T)ConverterFunctions.ConversionRulesToObject[t](BinHex.Substring(i * size, size));
                }
                catch (Exception)
                {
                    Console.WriteLine("[HEX HELPER]: Could not convert to Span. Hex String not in correct format: {0}", BinHex);
                }
            }
            return bytes.AsSpan();
        }
    }
}
