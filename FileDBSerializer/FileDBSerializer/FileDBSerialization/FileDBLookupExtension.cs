using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    public static class FileDBLookupExtension
    {
        public static T SelectNodes<T>(this IEnumerable<FileDBNode> Collection, String Lookup) where T: IEnumerable<FileDBNode>, new()
        {
            var resultList= new T();
            var Next = GetNextNodeName(Lookup, out Lookup);

            //get the current results
            var tempResults = Collection.Where(node => node.GetName().Equals(Next));

            //if we are not yet at the lookups end, we have to search the children and append the result of that.
            if (!Next.Equals(""))
            {
                foreach (FileDBNode x in tempResults)
                {
                    if (x is Tag)
                    {
                        resultList.Concat(((Tag)x).SelectNodes(Next));
                    }
                }
            }
            //if we are at the final statement, we need to concat the result.
            else
            {
                resultList.Concat(tempResults);
            }
            return resultList;
        }

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
            }

            Remaining = String.Empty;
            return String.Empty;
        }
    }
}
