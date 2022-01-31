using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBReader.src.XmlRepresentation
{
    /// <summary>
    /// Provides functions that can be used to replace invalid tag names like "Delayed Construction".
    /// </summary>
    public static class InvalidTagNameHelper
    {
        public static Dictionary<string, string> ReplaceOperations = new Dictionary<string, string>();

        public static void BuildAndAddReplaceOps(IEnumerable<String> ReplaceOps)
        {
            if (ReplaceOps.Count() % 2 != 0)
            {
                Console.WriteLine("[INVALID TAGNAMES]: Your last Element ({0}) is ignored. Can only construct ReplaceOperations from an even number of string arguments.", ReplaceOps.ElementAt(ReplaceOps.Count() -1));
            }
            for (int i = 0; i < ReplaceOps.Count()-1; i += 2)
            {
                AddReplaceOp(ReplaceOps.ElementAt(i), ReplaceOps.ElementAt(i + 1));
            }
        }

        public static void AddReplaceOp(String Key, String Value)
        {
            try {
                ReplaceOperations.SafeAdd(Key, Value);
            } catch {
                Console.WriteLine("[INVALID TAGNAMES]: Replace Operation {0} -> {1} is ignored: Cannot add an Item with duplicate key or value");
            }
        }

        public static void RemoveReplaceOp(String Key)
        {
            ReplaceOperations.Remove(Key);
        }

        public static void RegisterReplaceOperations(Dictionary<String, String> values)
        {
            foreach (KeyValuePair<string, string> pair in values)
            {
                AddReplaceOp(pair.Key, pair.Value);
            }
        }

        public static void UnregisterReplaceOperations(Dictionary<String, String> values)
        {
            foreach (KeyValuePair<string, string> pair in values)
            {
                RemoveReplaceOp(pair.Key);
            }
        }

        public static String GetCorrection(String s)
        {
            if (ReplaceOperations.Count > 0 && ReplaceOperations.TryGetValue(s, out var fuck))
            {
                return fuck;
            }
            else return s;
        }

        public static string GetReverseCorrection(String s)
        {
            if (ReplaceOperations.Count > 0 && ReplaceOperations.TryGetKey(s, out var fuck))
            {
                return fuck;
            }
            else return s;
        }

        public static void Reset()
        {
            ReplaceOperations.Clear(); 
        }
    }
}
