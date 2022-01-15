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
        private BinaryWriter writer;

        public FileDBWriter_V2(BinaryWriter _writer)
        {
            writer = _writer;
        }

        public void WriteAttrib(Attrib a)
        {
            WriteNodeID(a);
            writer.Write(ToByteBlocks(a.Content, a.Bytesize));
        }

        public void WriteTag(Tag t)
        {
            WriteNodeID(t);
            this.WriteNodeCollection(t.Children);
            WriteNodeTerminator();
        }

        public int WriteDictionary(Dictionary<ushort, string> dict)
        {
            int Offset = (int)writer.BaseStream.Position;
            writer.Write(dict.Count);
            int bytesWritten = 4;
            foreach (KeyValuePair<ushort, String> k in dict)
            {
                writer.Write(k.Key);
                bytesWritten += 2;
            }
            foreach (KeyValuePair<ushort, String> k in dict)
            {
                bytesWritten += writer.WriteString0(k.Value);
            }

            //fill each dictionary size up to a multiple of 8.
            int ToWrite = FileDBDocument_V2.GetBlockSpace(bytesWritten) - bytesWritten;
            var AddBytes = new byte[ToWrite];
            writer.Write(AddBytes);

            writer.Flush();
            return Offset;
        }

        public void WriteMagicBytes()
        {
            writer.Write(8);
            writer.Write(-2);
        }

        public void WriteTagSection(TagSection tagSection)
        {
            int TagsOffset = WriteDictionary(tagSection.Tags);
            int AttribsOffset = WriteDictionary(tagSection.Attribs);
            writer.Write(TagsOffset);
            writer.Write(AttribsOffset);
        }

        public void WriteNodeID(FileDBNode node)
        {
            writer.Write(node.Bytesize);
            writer.Write(node.ID);
        }

        public void WriteNodeTerminator()
        {
            writer.Write((Int64)0);
        }

        //Account for Attributes to be written in blocks.

        private byte[] ToByteBlocks(byte[] attrib, int bytesize)
        {
            int ContentSize = FileDBDocument_V2.GetBlockSpace(bytesize);
            Array.Resize<byte>(ref attrib, ContentSize);
            return attrib;
        }
    }
}
