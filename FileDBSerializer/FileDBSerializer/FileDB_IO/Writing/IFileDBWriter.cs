using System;
using System.Collections.Generic;
using System.IO;

namespace FileDBSerializing
{
    public interface IFileDBWriter
    {
        //has to be public for extension methods, meh
        public BinaryWriter Writer { get; }

        void WriteTag(Tag t);
        void WriteAttrib(Attrib a);


        #region Tag Section
        void RemoveNonesAndWriteTagSection(IFileDBDocument forDocument);
        (int, int) WriteTagSection(TagSection tagSection);
        void WriteNodeCountSection(int nodeCount);
        void WriteTagOffsets(int tagOffset, int attribOffset);
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns>The offset to the dictionary in the current writing stream</returns>
        int WriteDictionary(Dictionary<ushort, String> dict);

        void WriteMagicBytes();
        void WriteNodeTerminator();
    }

    public static class IFileDBWriterExtensions
    {
        public static void WriteNode(this IFileDBWriter filedbwriter, FileDBNode n)
        {
            if (n.NodeType == FileDBNodeType.Tag)
                filedbwriter.WriteTag((Tag)n);
            else if (n.NodeType == FileDBNodeType.Attrib)
                filedbwriter.WriteAttrib((Attrib)n);
        }

        public static void WriteNodeCollection(this IFileDBWriter filedbwriter, IEnumerable<FileDBNode> collection)
        {
            foreach (FileDBNode n in collection)
            {
                filedbwriter.WriteNode(n);
            }
        }

        public static void RemoveNonesAndWriteTagSection(this IFileDBWriter filedbwriter, TagSection tagSection, int nodeCount)
        {
            tagSection.Tags.Remove(1);
            tagSection.Attribs.Remove(32768);

            (int tagOffset, int attribOffset) = filedbwriter.WriteTagSection(tagSection);

            tagSection.Tags.Add(1, "None");
            tagSection.Attribs.Add(32768, "None");

            filedbwriter.WriteNodeCountSection(nodeCount);
            filedbwriter.WriteTagOffsets(tagOffset, attribOffset);
        }

        public static void Flush(this IFileDBWriter writer)
        {
            writer.Writer?.Flush();
        }
    }
}
