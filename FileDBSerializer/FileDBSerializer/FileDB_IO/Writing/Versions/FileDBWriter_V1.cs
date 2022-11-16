using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    internal class FileDBWriter_V1 : IFileDBWriter
    {
        public BinaryWriter Writer { get; }

        public FileDBWriter_V1(Stream s)
        {
            Writer = new BinaryWriter(s);
        }

        public void WriteNodeID(FileDBNode n)
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
        public void RemoveNonesAndWriteTagSection(IFileDBDocument forDocument)
        {
            TagSection tagSection = forDocument.Tags;

            tagSection.Tags.Remove(1);
            tagSection.Attribs.Remove(32768);

            (int tagOffset, int attribOffset) = this.WriteTagSection(tagSection);
            this.WriteTagOffsets(tagOffset, attribOffset);

            tagSection.Tags.Add(1, "None");
            tagSection.Attribs.Add(32768, "None");
        }

        public (int, int) WriteTagSection(TagSection tagSection)
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

        public int WriteDictionary(Dictionary<ushort, string> dict)
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
