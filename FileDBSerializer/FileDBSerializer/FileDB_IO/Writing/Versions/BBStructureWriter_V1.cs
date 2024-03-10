using FileDBSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    internal class BBStructureWriter_V1 : IBBStructureWriter
    {
        public BinaryWriter Writer { get; }

        public BBStructureWriter_V1(Stream s)
        {
            Writer = new BinaryWriter(s);
        }

        public void WriteNodeID(BBNode n)
        {
            Writer!.Write((ushort)n.ID);
        }

        public void WriteAttrib(Attrib a)
        {
            WriteNodeID(a);
            Writer!.Write7BitEncodedInt(a.Bytesize);
            Writer.Write(a.Content);
        }

        public void WriteTag(Tag t)
        {
            WriteNodeID(t);
            this.WriteNodeCollection(t.Children);
            WriteNodeTerminator();
        }

        #region Tag Section
        public void WriteTagSection(BBDocument forDocument)
        {
            TagSection tagSection = forDocument.TagSection;

            (int tagOffset, int attribOffset) = this.WriteTagsAndAttribs(tagSection);
            this.WriteTagOffsets(tagOffset, attribOffset);
        }

        public (int, int) WriteTagsAndAttribs(TagSection tagSection)
        {
            int offset = WriteDictionary(tagSection.Tags);
            WriteDictionary(tagSection.Attribs);

            return (offset, 0);
        }

        public void WriteTagOffsets(int tagOffset, int attribOffset)
        {
            Writer!.Write(tagOffset);
        }

        public void WriteNodeCountSection(int nodeCount)
        {
            return;
        }
        #endregion

        public int WriteDictionary(IReadOnlyDictionary<ushort, string> dict)
        {
            int offset = (int)Writer!.Position();
            Writer!.Write7BitEncodedInt(dict.Count);
            foreach (KeyValuePair<ushort, String> k in dict)
            {
                Writer.WriteString0(k.Value);
                Writer.Write(k.Key);
            }
            return offset;
        }

        public void WriteMagicBytes() 
        { 
        
        }

        public void WriteNodeTerminator()
        {
            Writer!.Write((Int16)0);
        }
    }
}
