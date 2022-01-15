using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing.LookUps
{
    public static class FileDBDocumentLookupExtension
    {
        public static IEnumerable<FileDBNode> SelectNodes(this IFileDBDocument document, String Lookup)
        {
            return document.Roots.SelectNodes(Lookup);
        }

        public static FileDBNode SelectSingleNode(this IFileDBDocument document, String Lookup)
        {
            return document.Roots.SelectSingleNode(Lookup);
        }

        public static IEnumerable<FileDBNode> SelectNodes(this Tag tag, String Lookup)
        {
            return tag.Children.SelectNodes(Lookup);
        }

        public static FileDBNode SelectSingleNode(this Tag tag, String Lookup)
        {
            return tag.Children.SelectSingleNode(Lookup);
        }
    }
}
