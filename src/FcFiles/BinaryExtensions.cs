using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FileDBReader
{

    public static class BinaryExtensions
    {
        #region Methods

        public static string ToHexString(this ReadOnlySpan<byte> Bytes)
        {
            StringBuilder Result = new StringBuilder(Bytes.Length * 2);
            string HexAlphabet = "0123456789ABCDEF";

            foreach (var B in Bytes)
            {
                Result.Append(HexAlphabet[B >> 4]);
                Result.Append(HexAlphabet[B & 0xF]);
            }

            return Result.ToString();
        }

        public static bool TryGetKey(this Dictionary<string, string> dict, String Value, out String result)
        {
            if (dict.ContainsValue(Value))
            {
                result = dict.First(x => x.Value.Equals(Value)).Key;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Adds an Element to a Dictionary under the condition that both value and key are unique.
        /// </summary>
        /// <typeparam name="T1">Key Type</typeparam>
        /// <typeparam name="T2">Value Type</typeparam>
        /// <param name="dict"></param>
        /// <param name="Key">Key to be added</param>
        /// <param name="Value">Value to be added</param>
        /// <exception cref="ArgumentException">If The Dictionary already contains the Value</exception>
        public static void SafeAdd<T1, T2>(this Dictionary<T1, T2> dict, T1 Key, T2 Value)
        {
            if (dict.ContainsValue(Value)) throw new ArgumentException();
            dict.Add(Key, Value);
        }

        public static IEnumerable<XmlNode> FilterOut(this XmlNodeList Base, XmlNodeList ToFilter)
        {
            //ToFilter should get removed from Base
            var castedBase = Base.Cast<XmlNode>();
            var castedToFilter = ToFilter.Cast<XmlNode>();
            //Xpath 2.0 support when, jesus this exists since 2003
            return castedBase.Except(castedToFilter);
        }
        #endregion Methods
    }
}