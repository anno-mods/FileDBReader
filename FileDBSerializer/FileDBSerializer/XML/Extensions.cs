using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoMods.BBDom.XML
{
    public static class DictionaryExtensions
    {
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

    }
}
