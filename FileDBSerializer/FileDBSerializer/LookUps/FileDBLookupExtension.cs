using System;
using System.Collections.Generic;
using System.Linq;

namespace AnnoMods.BBDom.LookUps
{
    public static class FileDBLookupExtension
    {
        //Delegate for LookupFilters.
        public delegate bool LookupCondition(BBNode node);

        public static IEnumerable<BBNode> SelectNodes(this IEnumerable<BBNode> Collection, String Lookup)
        {
            return SelectNodes(Collection, Lookup, node => true);
        }

        public static BBNode? SelectSingleNode(this IEnumerable<BBNode> Collection, String Lookup)
        {
            return SelectSingleNode(Collection, Lookup, node => true);
        }

        public static Tag? SelectSingleTag(this IEnumerable<BBNode> Collection, String Lookup)
        {
            return SelectSingleNode(Collection, Lookup, node => node is Tag) as Tag;
        }

        public static Attrib? SelectSingleAttrib(this IEnumerable<BBNode> Collection, String Lookup)
        {
            return SelectSingleNode(Collection, Lookup, node => node is Attrib) as Attrib;
        }

        #region LookupMethods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Collection"></param>
        /// <param name="Lookup"></param>
        /// <param name="condition"></param>
        /// <returns></returns>

        public static IEnumerable<BBNode> SelectNodes(this IEnumerable<BBNode> Collection, String Lookup, LookupCondition condition)
        {
            IEnumerable<BBNode> resultList= new List<BBNode>();
            var Next = GetNextNodeName(Lookup, out Lookup);

            //get the current results
            var tempResults = Collection.Where(node => node.GetName().Equals(Next));

            //if we are not yet at the lookups end, we have to search the children and append the result of that.
            if (!Lookup.Equals(""))
            {
                foreach (BBNode x in tempResults)
                {
                    if (x is Tag && condition(x))
                    {
                        resultList = resultList.Union(((Tag)x).SelectNodes(Lookup));
                    }
                }
            }
            //if we are at the final statement, we need to concat the result.
            else
            {
                resultList = resultList.Union(tempResults);
            }
            return resultList;
        }

        /// <summary>
        /// Selects a single FileDBNode in a FileDBNode Enumerable.
        /// </summary>
        /// <param name="Collection"></param>
        /// <param name="Lookup">Lookup Path of the node</param>
        /// <param name="condition">Condition matching the delegate type LookupConditon. This condition is matched only for the resulting nodes (last element of the lookup path)!.</param>
        /// <returns>The first FileDBNode matching the given condition and Lookup, or null, if no such node is found.</returns>
        public static BBNode? SelectSingleNode(this IEnumerable<BBNode> Collection, String Lookup, LookupCondition condition)
        {
            try
            {
                var Next = GetNextNodeName(Lookup, out var RemainingLookup);
                //we are not at the end of the path -> lookup tags only 
                if (!RemainingLookup.Equals(""))
                {
                    Tag tempResult = (Tag)Collection.First(node => node.GetName().Equals(Next) && node is Tag);
                    return tempResult.Children.SelectSingleNode(RemainingLookup, condition);
                }
                //we are at the end of the path
                else
                {
                    return Collection.First(node => node.GetName().Equals(Next) && condition(node));
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("[NODE LOOKUP]: Lookup failed: {0}. No nodes found, returned null.", Lookup);
                return null; 
            }
        }

        #endregion

        private static String GetNextNodeName(String LookupPath, out String Remaining)
        {
            if (!String.IsNullOrWhiteSpace(LookupPath))
            {
                int charLocation = LookupPath.IndexOf("/", StringComparison.Ordinal);
                if (charLocation > 0)
                {
                    Remaining = LookupPath.Substring(charLocation + 1);
                    return LookupPath.Substring(0, charLocation);
                }
                else 
                { 
                    Remaining = String.Empty;
                    return LookupPath;
                }
            }
            Remaining = String.Empty;
            return String.Empty;
        }
    }
}
