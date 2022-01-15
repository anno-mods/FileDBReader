using FileDBSerializing.LookUps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    [DebuggerDisplay("[FileDB_Document: Version = 1, Count = {ELEMENT_COUNT}]")]
    public class FileDBDocument_V1 : IFileDBDocument
    {
        //PUBLIC MEMBERS
        public List<FileDBNode> Roots { get; set; }
        public TagSection Tags { get; set; }
        public int ELEMENT_COUNT { get => Roots.Count; }
        public byte[] MAGIC_BYTES { get => _magic_bytes; }

        //Version 1 does not have magic bytes, so MAGIC_BYTE_COUNT is automatically 0!
        public int MAGIC_BYTE_COUNT { get => 0; }
        public int VERSION { get; }
        public int OFFSET_TO_OFFSETS { get => _offset_to_offsets; }
        public ushort MAX_TAG_ID { get => _max_tag_id; set => _max_tag_id = value; }
        public ushort MAX_ATTRIB_ID { get => _max_attrib_id; set => _max_attrib_id = value; }

        //INTERNAL MEMBERS
        internal static int _offset_to_offsets = 4;
        internal static byte[] _magic_bytes = new byte[0];
        private ushort _max_tag_id = 1; //0x01 0x00
        private ushort _max_attrib_id = 32768; //0x01 0x80

        //CONSTRUCTORS
        public FileDBDocument_V1()
        {
            Roots = new List<FileDBNode>();
            Tags = new TagSection();
            VERSION = 1;
        }
    }
}
