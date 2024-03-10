using System.Collections.Generic;
using System.IO;

namespace AnnoMods.BBDom.IO
{
    internal class BBStructureParser_V1 : IBBStructureParser
    {
        public BinaryReader Reader { get; }
        public BBDocument? TargetDocument { get; set; }

        public BBStructureParser_V1(Stream s)
        {
            Reader = new BinaryReader(s);
        }

        public byte[] ReadAttribContent(int bytesize)
        {
            return Reader!.ReadBytes(bytesize);
        }

        public Dictionary<ushort, string> ParseDictionary(int Offset)
        {
            Reader!.BaseStream.Position = Offset;
            Dictionary<ushort, string> dictionary = new Dictionary<ushort, string>();

            int Count = Reader.Read7BitEncodedInt();
            for (int i = 0; i < Count; i++)
            {
                string Name = Reader.ReadString0();
                var id = Reader.ReadUInt16();
                dictionary.Add(id, Name);
            }
            return dictionary;
        }

        public Attrib ReadAttrib(int bytesize, int ID, Tag ParentTag)
        {
            Attrib a = this.CreateAttrib(bytesize, ID, TargetDocument!);
            a.Parent = ParentTag;
            return a;
        }

        public States ReadNextOperation(out int _bytesize, out ushort _id)
        {
            ushort ID = Reader!.ReadUInt16();
            _id = ID;
            _bytesize = 0;
            States State = this.DetermineState(ID);
            if (State == States.Attrib)
            {
                _bytesize = Reader.Read7BitEncodedInt();
            }
            return State;
        }

        public Tag ReadTag(int ID, Tag ParentTag)
        {
            return new Tag() { ID = ID, ParentDoc = TargetDocument!, Parent = ParentTag };
        }

        public TagSection ReadTagSection(int OffsetToOffsets)
        {
            Reader!.SetPosition(Reader!.BaseStream.Length - OffsetToOffsets);

            int TagsOffset = Reader.ReadInt32();
            var Tags = ParseDictionary(TagsOffset);
            int AttribOffset = (int)Reader.Position();
            var Attribs = ParseDictionary(AttribOffset);
            return new TagSection(Tags, Attribs);
        }
    }
}
