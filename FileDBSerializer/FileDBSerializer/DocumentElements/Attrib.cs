using System;
using System.Diagnostics;
using System.IO;

namespace FileDBSerializing
{
    [DebuggerDisplay("[FileDB_Attrib: ID = {ID}, Size = {Bytesize}]")]
    public class Attrib : FileDBNode
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

        public Attrib()
        {
            NodeType = FileDBNodeType.Attrib;
        }
        public override String GetID()
        {
            if (ParentDoc.Tags.Attribs.TryGetValue((ushort)ID, out string value))
                return value;
            else return "a_" + ID;
        }

        public override string GetName()
        {
            return ParentDoc.Tags.Attribs[(ushort)ID];
        }

        public MemoryStream ContentToStream()
        {
            return new MemoryStream(Content);
        }
    }
}
