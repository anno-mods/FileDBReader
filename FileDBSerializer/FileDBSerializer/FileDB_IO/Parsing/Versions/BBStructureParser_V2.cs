using FileDBSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    internal class BBStructureParser_V2 : IBBStructureParser
    {
        public BinaryReader Reader { get; }
        public BBDocument? TargetDocument { get; set; }

        public BBStructureParser_V2(Stream s)
        {
            Reader = new BinaryReader(s);
        }

        public Dictionary<ushort, string> ParseDictionary(int Offset)
        {
            Reader!.BaseStream.Position = Offset;
            Dictionary<ushort, string> dictionary = new Dictionary<ushort, string>();

            int Count = Reader.ReadInt32();
            ushort[] IDs = new ushort[Count];

            for (int i = 0; i < Count; i++)
            {
                IDs[i] = Reader.ReadUInt16();
            }
            for (int i = 0; i < Count; i++)
            {
                String name = Reader.ReadString0();
                dictionary.Add(IDs[i], name);
            }
            return dictionary;
        }

        public Attrib ReadAttrib(int bytesize, int ID, Tag ParentTag)
        {
            Attrib a = this.CreateAttrib(bytesize, ID, TargetDocument!);
            a.Parent = ParentTag;
            return a;
        }

        public byte[] ReadAttribContent(int bytesize)
        {
            int ContentSize = MemoryBlocks.GetBlockSpace(bytesize, Versioning.GetBlockSize(BBDocumentVersion.V2));
            byte[] Content = Reader!.ReadBytes(ContentSize);
            Array.Resize<byte>(ref Content, bytesize);
            return Content;
        }

        public States ReadNextOperation(out int _bytesize, out ushort _id)
        {
            //typecast is not nice. bb fix your shitty compression. 
            int bytesize = Reader!.ReadInt32();
            ushort ID = (ushort)Reader.ReadInt32();
            _bytesize = bytesize;
            _id = ID;
            return this.DetermineState(ID);
        }

        public Tag ReadTag(int ID, Tag ParentTag)
        {
            return new Tag() { ID = ID, ParentDoc = TargetDocument!, Parent = ParentTag };
        }

        public TagSection ReadTagSection(int OffsetToOffsets)
        {
            Reader!.SetPosition(Reader!.BaseStream.Length - OffsetToOffsets);

            int TagsOffset = Reader.ReadInt32();
            int AttribsOffset = Reader.ReadInt32();
            return new TagSection(
                /*Tags*/    ParseDictionary(TagsOffset),
                /*Attribs*/ ParseDictionary(AttribsOffset)
            );
        }
    }
}
