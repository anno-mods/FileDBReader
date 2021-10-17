using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBReader.src
{
    /// <summary>
    /// Provides functions that can be used to replace invalid tag names like "Delayed Construction". Ignores duplicate values for now
    /// </summary>
    public static class InvalidTagNameHelper
    {
        public static Dictionary<string, string> ReplaceOperations;

        /// <summary>
        /// Renames Values in a Dictionary. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InputDictionary"></param>
        /// <param name="Reverse">Call with true to use an inversion of the dictionary during replacement.</param>
        /// <returns></returns>
        public static Dictionary<T, string> RenameValuesInDictionary<T>(Dictionary<T, string> InputDictionary, bool Reverse) where T: struct
        {
            if (Reverse)
                ReplaceOperations.Reverse(); 

            foreach (var pair in InputDictionary)
            {
                if (ReplaceOperations.ContainsKey(pair.Value))
                {
                    InputDictionary[pair.Key] = ReplaceOperations[pair.Value];
                }
            }

            if (Reverse)
                ReplaceOperations.Reverse(); 
            return InputDictionary;
        }

        public static Dictionary<string, T> RenameKeysInDictionary<T>(Dictionary<string, T> InputDictionary, bool Reverse) where T : struct
        {
            var result = new Dictionary<string, T>(); 
            if (Reverse)
                ReplaceOperations = ReplaceOperations.Reverse();

            foreach (var pair in InputDictionary)
            {
                if (ReplaceOperations.ContainsKey(pair.Key))
                {
                    result.Add(ReplaceOperations[pair.Key], pair.Value);
                }
                else
                {
                    result.Add(pair.Key, pair.Value);
                }
            }

            if (Reverse)
                ReplaceOperations = ReplaceOperations.Reverse();
            return result;
        }

        public static void BuildDictionary(IEnumerable<String> InputEnumerable)
        {
            //if there is an uneven number of strings in the enumerable, cut the last one.
            int MaxUsableIndex = InputEnumerable.Count() - InputEnumerable.Count() % 2;

            var ReplaceOperations = new Dictionary<string, string>();
            for (int i = 0; i < MaxUsableIndex; i += 2)
            {
                ReplaceOperations.Add(InputEnumerable.ElementAt<string>(i), InputEnumerable.ElementAt<string>(i + 1));
            }
        }

        public static void Reset()
        {
            ReplaceOperations = null; 
        }

        /// <summary>
        /// Extension Method for a generic Dictionary to reverse it. copied from https://stackoverflow.com/questions/22595655/how-to-do-a-dictionary-reverse-lookup/22595707
        /// </summary>
        /// <returns></returns>
        public static Dictionary<TValue, TKey> Reverse<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            var dictionary = new Dictionary<TValue, TKey>();
            foreach (var entry in source)
            {
                if (!dictionary.ContainsKey(entry.Value))
                    dictionary.Add(entry.Value, entry.Key);
            }
            return dictionary;
        }
        /// <summary>
        /// Extension Method to rename a Key in a Dictionary. copied from https://stackoverflow.com/questions/6499334/best-way-to-change-dictionary-key/6499344
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="fromKey"></param>
        /// <param name="toKey"></param>
        public static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey fromKey, TKey toKey)
        {
            TValue value = dic[fromKey];
            dic.Remove(fromKey);
            dic[toKey] = value;
        }
    }
}
