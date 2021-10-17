using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;

namespace FileDBReader
{
    /// <summary>
    /// Static Library of helpers
    /// </summary>
    static class HexHelper
    {
        public static string FromHexString(string hexString, Encoding encoding)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                try
                {
                    bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }
                catch (Exception e) {
                    Console.WriteLine("Hex String not in correct format: {0}", hexString);
                }
            }
            return encoding.GetString(bytes);
        }

        public static bool ToBool(String hexString)
        {
            return hexString.Equals("01");
        }

        /// <summary>
        /// Floats are big endian meh
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static float ToFloat(String hexString)
        {
            uint num = uint.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);

            byte[] floatVals = BitConverter.GetBytes(num);
            float f = BitConverter.ToSingle(floatVals, 0);
            return f;
        }

        public static String flip(String hex)
        {
            String s = "";
            for (int i = hex.Length; i > 1; i -= 2)
            {
                s += hex.Substring(i - 2, 2);
            }
            return s;
        }

        public static IEnumerable<XmlNode> ExceptNodelists(XmlNodeList Base, XmlNodeList ToFilter)
        {
            //ToFilter should get removed from Base
            var castedBase = new List<XmlNode>();
            var castedToFilter = new List<XmlNode>();

            //Xpath 2.0 support when, jesus this exists since 2003
            foreach (XmlNode node in Base)
            {
                castedBase.Add(node);
            }
            foreach (XmlNode node in ToFilter)
            {
                castedToFilter.Add(node);
            }
            return castedBase.Except(castedToFilter);
        }

        public static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        public static string AddSuffix(string filename, string suffix)
        {
            string fDir = Path.GetDirectoryName(filename);
            string fName = Path.GetFileNameWithoutExtension(filename);
            string fExt = Path.GetExtension(filename);
            return Path.Combine(fDir, String.Concat(fName, suffix, fExt));
        }

        public static Span<T> toSpan<T>(String s) where T : struct
        {
            int size = Marshal.SizeOf(default(T)) * 2;
            var bytes = new T[s.Length / size];
            Type t = typeof(T);
            for (var i = 0; i < bytes.Length; i++)
            {
                try
                {
                    bytes[i] = (T)ConverterFunctions.ConversionRulesToObject[t](s.Substring(i * size, size));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Hex String not in correct format: {0}", s);
                }
            }
            return bytes.AsSpan();
        }
        public static byte[] StreamToByteArray(Stream s)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                s.Position = 0;
                s.CopyTo(ms);
                return ms.ToArray();
            }
        }

        //copied from https://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array/321404 because why not
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        //using String.Join for performance optimization over for loops.
        public static String Join<T>(String BinaryData) where T : struct
        {
            var span = HexHelper.toSpan<T>(BinaryData).ToArray();
            return String.Join<T>(" ", span);
        }

    }
}
