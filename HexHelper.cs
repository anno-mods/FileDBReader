using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.IO;

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
            return encoding.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"
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

    }
}
