using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    internal class FileDBParser_V1 : IFileDBParser
    {
        private BinaryReader reader;
        private IFileDBDocument TargetDocument; 

        public FileDBParser_V1(BinaryReader r, IFileDBDocument doc)
        {
            reader = r;
            TargetDocument = doc;
        }

        public byte[] ReadAttribContent(int bytesize)
        {
            return reader.ReadBytes(bytesize);
        }

        public Dictionary<ushort, string> ParseDictionary(int Offset)
        {
            reader.BaseStream.Position = Offset;
            Dictionary<ushort, String> dictionary = new Dictionary<ushort, String>();

            int Count = reader.Read7BitEncodedInt();
            for (int i = 0; i < Count; i++)
            {
                String Name = reader.ReadString0();
                var id = reader.ReadUInt16();
                dictionary.Add(id, Name);
            }
            return dictionary;
        }

        public Attrib ReadAttrib(int bytesize, int ID, Tag ParentTag)
        {
            Attrib a = this.CreateAttrib(bytesize, ID, TargetDocument);
            a.Parent = ParentTag;
            return a;
        }

        public States ReadNextOperation(out int _bytesize, out ushort _id)
        {
            ushort ID = reader.ReadUInt16();
            _id = ID;
            _bytesize = 0;
            States State = this.DetermineState(ID);
            if (State == States.Attrib)
            {
                _bytesize = reader.Read7BitEncodedInt();
            }
            return State;
        }

        public Tag ReadTag(int ID, Tag ParentTag)
        {
            return new Tag() { ID = ID, ParentDoc = TargetDocument, Parent = ParentTag };
        }

        public TagSection ReadTagSection(int OffsetToOffsets)
        {
            reader.SetPosition(reader.BaseStream.Length - OffsetToOffsets);

            int TagsOffset = reader.ReadInt32();
            var Tags = ParseDictionary(TagsOffset);
            int AttribOffset = (int)reader.Position();
            var Attribs = ParseDictionary(AttribOffset);
            return new TagSection(Tags, Attribs);
        }
    }
}
