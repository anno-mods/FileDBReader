using System;
using System.Collections.Generic;
using static AnnoMods.BBDom.LookUps.FileDBLookupExtension;

namespace AnnoMods.BBDom.LookUps
{
    public static class FileDBDocumentLookupExtension
    {
        #region BBDocumentLookups
        public static IEnumerable<BBNode> SelectNodes(this BBDocument document, String Lookup)
        {
            return document.Roots.SelectNodes(Lookup);
        }

        //Overload with Condition
        public static IEnumerable<BBNode> SelectNodes(this BBDocument document, String Lookup, LookupCondition condition)
        {
            return document.Roots.SelectNodes(Lookup, condition);
        }

        public static BBNode? SelectSingleNode(this BBDocument document, String Lookup)
        {
            return document.Roots.SelectSingleNode(Lookup);
        }

        //Overload with Condition
        public static BBNode? SelectSingleNode(this BBDocument document, String Lookup, LookupCondition condition)
        {
            return document.Roots.SelectSingleNode(Lookup, condition);
        }
        #endregion

        #region TagLookups
        public static IEnumerable<BBNode> SelectNodes(this Tag tag, String Lookup)
        {
            return tag.Children.SelectNodes(Lookup);
        }

        public static IEnumerable<BBNode> SelectNodes(this Tag tag, String Lookup, LookupCondition condition)
        {
            return tag.Children.SelectNodes(Lookup, condition);
        }

        public static BBNode? SelectSingleNode(this Tag tag, String Lookup)
        {
            return tag.Children.SelectSingleNode(Lookup);
        }

        public static BBNode? SelectSingleNode(this Tag tag, String Lookup, LookupCondition condition)
        {
            return tag.Children.SelectSingleNode(Lookup, condition);
        }
        #endregion
    }
}
