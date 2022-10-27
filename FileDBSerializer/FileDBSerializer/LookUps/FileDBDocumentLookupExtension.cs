using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FileDBSerializing.LookUps.FileDBLookupExtension;

namespace FileDBSerializing.LookUps
{
    public static class FileDBDocumentLookupExtension
    {
        #region IFileDBDocumentLookups
        public static IEnumerable<FileDBNode> SelectNodes(this IFileDBDocument document, String Lookup)
        {
            return document.Roots.SelectNodes(Lookup);
        }

        //Overload with Condition
        public static IEnumerable<FileDBNode> SelectNodes(this IFileDBDocument document, String Lookup, LookupCondition condition)
        {
            return document.Roots.SelectNodes(Lookup, condition);
        }

        public static FileDBNode? SelectSingleNode(this IFileDBDocument document, String Lookup)
        {
            return document.Roots.SelectSingleNode(Lookup);
        }

        //Overload with Condition
        public static FileDBNode? SelectSingleNode(this IFileDBDocument document, String Lookup, LookupCondition condition)
        {
            return document.Roots.SelectSingleNode(Lookup, condition);
        }
        #endregion

        #region TagLookups
        public static IEnumerable<FileDBNode> SelectNodes(this Tag tag, String Lookup)
        {
            return tag.Children.SelectNodes(Lookup);
        }

        public static IEnumerable<FileDBNode> SelectNodes(this Tag tag, String Lookup, LookupCondition condition)
        {
            return tag.Children.SelectNodes(Lookup, condition);
        }

        public static FileDBNode? SelectSingleNode(this Tag tag, String Lookup)
        {
            return tag.Children.SelectSingleNode(Lookup);
        }

        public static FileDBNode? SelectSingleNode(this Tag tag, String Lookup, LookupCondition condition)
        {
            return tag.Children.SelectSingleNode(Lookup, condition);
        }
        #endregion
    }
}
