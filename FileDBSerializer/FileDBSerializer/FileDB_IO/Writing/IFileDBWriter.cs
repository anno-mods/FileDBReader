using System;
using System.Collections.Generic;
using System.IO;

namespace FileDBSerializing
{
    internal interface IFileDBWriter
    {
        void WriteTag(Tag t);
        void WriteAttrib(Attrib a);
        void WriteTagSection(TagSection tagSection);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns>The offset to the dictionary in the current writing stream</returns>
        int WriteDictionary(Dictionary<ushort, String> dict);

        void WriteMagicBytes();
        void WriteNodeTerminator();
    }

    internal static class IFileDBWriterExtensions
    {
        internal static void WriteNode(this IFileDBWriter filedbwriter, FileDBNode n)
        {
            if (n.NodeType == FileDBNodeType.Tag)
                filedbwriter.WriteTag((Tag)n);
            else if (n.NodeType == FileDBNodeType.Attrib)
                filedbwriter.WriteAttrib((Attrib)n);
        }

        internal static void WriteNodeCollection(this IFileDBWriter filedbwriter, IEnumerable<FileDBNode> collection)
        {
            foreach (FileDBNode n in collection)
            {
                filedbwriter.WriteNode(n);
            }
        }

        internal static void RemoveNonesAndWriteTagSection(this IFileDBWriter filedbwriter, TagSection tagSection)
        {
            tagSection.Tags.Remove(1);
            tagSection.Attribs.Remove(32768);

            filedbwriter.WriteTagSection(tagSection);

            tagSection.Tags.Add(1, "None");
            tagSection.Attribs.Add(32768, "None");
        }
    }
}
