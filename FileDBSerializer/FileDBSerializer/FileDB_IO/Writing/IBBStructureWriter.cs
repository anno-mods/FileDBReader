using FileDBSerializer;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileDBSerializing
{
    public interface IBBStructureWriter
    {
        //has to be public for extension methods, meh
        public BinaryWriter Writer { get; }

        void WriteTag(Tag t);
        void WriteAttrib(Attrib a);


        #region Tag Section
        void WriteTagSection(BBDocument forDocument);
        (int, int) WriteTagsAndAttribs(TagSection tagSection);
        void WriteNodeCountSection(int nodeCount);
        void WriteTagOffsets(int tagOffset, int attribOffset);
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns>The offset to the dictionary in the current writing stream</returns>
        int WriteDictionary(IReadOnlyDictionary<ushort, String> dict);

        void WriteMagicBytes();
        void WriteNodeTerminator();
    }

    public static class IFileDBWriterExtensions
    {
        public static void WriteNode(this IBBStructureWriter filedbwriter, BBNode n)
        {
            if (n.NodeType == FileDBNodeType.Tag)
                filedbwriter.WriteTag((Tag)n);
            else if (n.NodeType == FileDBNodeType.Attrib)
                filedbwriter.WriteAttrib((Attrib)n);
        }

        public static void WriteNodeCollection(this IBBStructureWriter filedbwriter, IEnumerable<BBNode> collection)
        {
            foreach (BBNode n in collection)
            {
                filedbwriter.WriteNode(n);
            }
        }

        public static void WriteTagSection(this IBBStructureWriter filedbwriter, TagSection tagSection, int nodeCount)
        {
            (int tagOffset, int attribOffset) = filedbwriter.WriteTagsAndAttribs(tagSection);

            filedbwriter.WriteNodeCountSection(nodeCount);
            filedbwriter.WriteTagOffsets(tagOffset, attribOffset);
        }

        public static void Flush(this IBBStructureWriter writer)
        {
            writer.Writer?.Flush();
        }
    }
}
