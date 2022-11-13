using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    internal class FileDBWriter_V2 : IFileDBWriter
    {
        public BinaryWriter Writer { get; }

        private static int MemBlockSize = FileDBDocument_V2.ATTRIB_BLOCK_SIZE;

        public FileDBWriter_V2(Stream s)
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

        public int WriteDictionary(Dictionary<ushort, string> dict)
        {
            int Offset = (int)Writer!.BaseStream.Position;
            Writer.Write(dict.Count);
            int bytesWritten = 4;
            foreach (KeyValuePair<ushort, String> k in dict)
            {
                Writer.Write(k.Key);
                bytesWritten += 2;
            }
            foreach (KeyValuePair<ushort, String> k in dict)
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
            Writer!.Write(Versioning.GetMagicBytes(FileDBDocumentVersion.Version2));
        }

        public void WriteTagSection(TagSection tagSection)
        {
            int TagsOffset = WriteDictionary(tagSection.Tags);
            int AttribsOffset = WriteDictionary(tagSection.Attribs);
            Writer!.Write(TagsOffset);
            Writer.Write(AttribsOffset);
        }

        public void WriteNodeID(FileDBNode node)
        {
            Writer!.Write(node.Bytesize);
            Writer.Write(node.ID);
        }

        public void WriteNodeTerminator()
        {
            Writer!.Write((Int64)0);
        }

        //Account for Attributes to be written in blocks.

        private byte[] ToByteBlocks(byte[] attrib, int bytesize)
        {
            int ContentSize = MemoryBlocks.GetBlockSpace(bytesize, MemBlockSize);
            Array.Resize<byte>(ref attrib, ContentSize);
            return attrib;
        }

    }
}
