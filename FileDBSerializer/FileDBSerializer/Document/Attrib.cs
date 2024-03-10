using AnnoMods.BBDom.IO;
using System;
using System.Diagnostics;
using System.IO;

namespace AnnoMods.BBDom
{
    [DebuggerDisplay("[FileDB_Attrib: ID = {ID}, Name = {Name}, Size = {Bytesize}]")]
    public class Attrib : BBNode
    {
        private byte[] _content;
        public byte[] Content
        {
            set
            {
                _content = value;
                //sync bytesize and length of content
                Bytesize = value.Length;
            }
            get
            {
                return _content;
            }
        }

        internal Attrib()
        {
            NodeType = FileDBNodeType.Attrib;
        }
        public override string GetNameWithFallback() => ParentDoc.TagSection.GetAttribName(ID) ?? "a_" + ID;

        public override string GetName() => ParentDoc.TagSection.GetAttribName(ID)
            ?? throw new InvalidFileDBException($"ID {ID} does not correspond to a Name in this Documents Tags Section.");


        public MemoryStream ContentToStream()
        {
            return new MemoryStream(Content);
        }
    }
}
