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
        private BinaryWriter writer;

        public FileDBWriter_V1(BinaryWriter _writer)
        { 
            writer = _writer;
        }

        public void WriteNodeID(FileDBNode n)
        {
            writer.Write((ushort)n.ID);
        }

        public void WriteAttrib(Attrib a)
        {
            WriteNodeID(a);
            writer.Write7BitEncodedInt(a.Bytesize);
            writer.Write(a.Content);
        }

        public void WriteTag(Tag t)
        {
            WriteNodeID(t);
            this.WriteNodeCollection(t.Children);
            WriteNodeTerminator();
        }

        public void WriteTagSection(TagSection tagSection)
        {
            int offset = WriteDictionary(tagSection.Tags);
            WriteDictionary(tagSection.Attribs);
            writer.Write(offset);
        }

        public int WriteDictionary(Dictionary<ushort, string> dict)
        {
            int offset = (int)writer.Position();
            writer.Write7BitEncodedInt(dict.Count);
            foreach (KeyValuePair<ushort, String> k in dict)
            {
                writer.WriteString0(k.Value);
                writer.Write(k.Key);
            }
            return offset;
        }

        public void WriteMagicBytes() 
        { 
        
        }

        public void WriteNodeTerminator()
        {
            writer.Write((Int16)0);
        }
    }
}
