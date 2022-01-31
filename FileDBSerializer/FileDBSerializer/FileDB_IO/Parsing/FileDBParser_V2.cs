using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    internal class FileDBParser_V2 : IFileDBParser
    {
        private BinaryReader reader;
        private IFileDBDocument TargetDocument;

        public FileDBParser_V2(BinaryReader _reader, IFileDBDocument doc)
        {
            reader = _reader;
            TargetDocument = doc;
        }

        public Dictionary<ushort, string> ParseDictionary(int Offset)
        {
            reader.BaseStream.Position = Offset;
            Dictionary<ushort, string> dictionary = new Dictionary<ushort, string>();

            int Count = reader.ReadInt32();
            ushort[] IDs = new ushort[Count];

            for (int i = 0; i < Count; i++)
            {
                IDs[i] = reader.ReadUInt16();
            }
            for (int i = 0; i < Count; i++)
            {
                String name = reader.ReadString0();
                dictionary.Add(IDs[i], name);
            }
            return dictionary;
        }

        public Attrib ReadAttrib(int bytesize, int ID, Tag ParentTag)
        {
            Attrib a = this.CreateAttrib(bytesize, ID, TargetDocument);
            a.Parent = ParentTag;
            return a;
        }

        public byte[] ReadAttribContent(int bytesize)
        {
            int ContentSize = FileDBDocument_V2.GetBlockSpace(bytesize);
            byte[] Content = reader.ReadBytes(ContentSize);
            Array.Resize<byte>(ref Content, bytesize);
            return Content;
        }

        public States ReadNextOperation(out int _bytesize, out ushort _id)
        {
            //typecast is not nice. bb fix your shitty compression. 
            int bytesize = reader.ReadInt32();
            ushort ID = (ushort)reader.ReadInt32();
            _bytesize = bytesize;
            _id = ID;
            return this.DetermineState(ID);
        }

        public Tag ReadTag(int ID, Tag ParentTag)
        {
            return new Tag() { ID = ID, ParentDoc = TargetDocument, Parent = ParentTag };
        }

        public TagSection ReadTagSection(int OffsetToOffsets)
        {
            reader.SetPosition(reader.BaseStream.Length - OffsetToOffsets);

            int TagsOffset = reader.ReadInt32();
            int AttribsOffset = reader.ReadInt32();
            return new TagSection(
                /*Tags*/    ParseDictionary(TagsOffset),
                /*Attribs*/ ParseDictionary(AttribsOffset)
            );
        }
    }
}
