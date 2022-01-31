using FileDBSerializing.LookUps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    [DebuggerDisplay("[FileDB_Document: Version = 2, Count = {ELEMENT_COUNT}]")]
    public class FileDBDocument_V2 : IFileDBDocument
    {
        //FIELDS 
        public List<FileDBNode> Roots { get; set; }
        public TagSection Tags { get; set; }
        public int ELEMENT_COUNT { get => Roots.Count; }
        public FileDBDocumentVersion VERSION { get; }
        public byte[] MAGIC_BYTES { get => _magic_bytes; }
        public int MAGIC_BYTE_COUNT { get => _magic_byte_count; }
        public int OFFSET_TO_OFFSETS { get => _offset_to_offsets; }
        public ushort MAX_TAG_ID { get => _max_tag_id; set => _max_tag_id = value; }
        public ushort MAX_ATTRIB_ID { get => _max_attrib_id; set => _max_attrib_id = value; }

        //INTERNAL MEMBERS: Just for Binding the public ones.

        internal static int ATTRIB_BLOCK_SIZE = 8;
        internal static int _offset_to_offsets = 16;
        internal static byte[] _magic_bytes = { 0x08, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0xFF, 0xFF };
        internal static int _magic_byte_count = _magic_bytes.Length;
        private ushort _max_tag_id = 1; //0x01 0x00
        private ushort _max_attrib_id = 32768; //0x01 0x80

        //CONSTRUCTORS
        public FileDBDocument_V2()
        {
            Roots = new List<FileDBNode>();
            Tags = new TagSection();
            VERSION = FileDBDocumentVersion.Version2;
        }

        //STATIC FUNCTIONS
        public static int GetBlockSpace(int bytesize)
        {
            return ((bytesize / FileDBDocument_V2.ATTRIB_BLOCK_SIZE) * FileDBDocument_V2.ATTRIB_BLOCK_SIZE + FileDBDocument_V2.ATTRIB_BLOCK_SIZE * Math.Clamp(bytesize % FileDBDocument_V2.ATTRIB_BLOCK_SIZE, 0, 1));
        }
    }
}
