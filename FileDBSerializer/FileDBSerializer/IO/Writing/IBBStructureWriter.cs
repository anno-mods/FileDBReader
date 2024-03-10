using System.Collections.Generic;
using System.IO;

namespace AnnoMods.BBDom.IO
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
        int WriteDictionary(IReadOnlyDictionary<ushort, string> dict);

        void WriteMagicBytes();
        void WriteNodeTerminator();
    }

    public static class IBBWriterExtensions
    {
        public static void WriteNode(this IBBStructureWriter BBwriter, BBNode n)
        {
            if (n.NodeType == BBNodeType.Tag)
                BBwriter.WriteTag((Tag)n);
            else if (n.NodeType == BBNodeType.Attrib)
                BBwriter.WriteAttrib((Attrib)n);
        }

        public static void WriteNodeCollection(this IBBStructureWriter BBwriter, IEnumerable<BBNode> collection)
        {
            foreach (BBNode n in collection)
            {
                BBwriter.WriteNode(n);
            }
        }

        public static void WriteTagSection(this IBBStructureWriter BBwriter, TagSection tagSection, int nodeCount)
        {
            (int tagOffset, int attribOffset) = BBwriter.WriteTagsAndAttribs(tagSection);

            BBwriter.WriteNodeCountSection(nodeCount);
            BBwriter.WriteTagOffsets(tagOffset, attribOffset);
        }

        public static void Flush(this IBBStructureWriter writer)
        {
            writer.Writer?.Flush();
        }
    }
}
