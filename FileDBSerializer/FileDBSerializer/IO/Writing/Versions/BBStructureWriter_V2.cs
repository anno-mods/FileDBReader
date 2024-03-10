using AnnoMods.BBDom;
using AnnoMods.BBDom.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoMods.BBDom.IO
{
    internal class BBStructureWriter_V2 : IBBStructureWriter
    {
        public BinaryWriter Writer { get; }

        private static int MemBlockSize = Versioning.GetBlockSize(BBDocumentVersion.V2);

        public BBStructureWriter_V2(Stream s)
        {
            Writer = new BinaryWriter(s);
        }

        public void WriteAttrib(Attrib a)
        {
            WriteNodeID(a);
            Writer!.Write(ToByteBlocks(a.Content, a.Bytesize));
        }

        public void WriteTag(Tag t)
        {
            WriteNodeID(t);
            this.WriteNodeCollection(t.Children);
            WriteNodeTerminator();
        }

        public int WriteDictionary(IReadOnlyDictionary<ushort, string> dict)
        {
            int Offset = (int)Writer!.BaseStream.Position;
            Writer.Write(dict.Count);
            int bytesWritten = 4;
            foreach (KeyValuePair<ushort, string> k in dict)
            {
                Writer.Write(k.Key);
                bytesWritten += 2;
            }
            foreach (KeyValuePair<ushort, string> k in dict)
            {
                bytesWritten += Writer.WriteString0(k.Value);
            }

            //fill each dictionary size up to a multiple of 8.
            int ToWrite = MemoryBlocks.GetBlockSpace(bytesWritten, MemBlockSize) - bytesWritten;
            var AddBytes = new byte[ToWrite];
            Writer.Write(AddBytes);

            Writer.Flush();
            return Offset;
        }

        public virtual void WriteMagicBytes()
        {
            Writer!.Write(Versioning.GetMagicBytes(BBDocumentVersion.V2));
        }


        #region Tag Section
        public virtual void WriteTagSection(BBDocument forDocument)
        {
            TagSection tagSection = forDocument.TagSection; ;

            (int tagOffset, int attribOffset) = WriteTagsAndAttribs(tagSection);
            WriteTagOffsets(tagOffset, attribOffset);
        }

        public (int, int) WriteTagsAndAttribs(TagSection tagSection)
        {
            int TagsOffset = WriteDictionary(tagSection.Tags);
            int AttribsOffset = WriteDictionary(tagSection.Attribs);

            return (TagsOffset, AttribsOffset);
        }

        public void WriteTagOffsets(int tagOffset, int attribOffset)
        {
            Writer!.Write(tagOffset);
            Writer!.Write(attribOffset);
        }

        public virtual void WriteNodeCountSection(int nodeCount)
        {
            return;
        }
        #endregion

        public void WriteNodeID(BBNode node)
        {
            Writer!.Write(node.Bytesize);
            Writer.Write(node.ID);
        }

        public void WriteNodeTerminator()
        {
            Writer!.Write((long)0);
        }

        //Account for Attributes to be written in blocks.

        private byte[] ToByteBlocks(byte[] attrib, int bytesize)
        {
            int ContentSize = MemoryBlocks.GetBlockSpace(bytesize, MemBlockSize);
            Array.Resize(ref attrib, ContentSize);
            return attrib;
        }
    }
}
