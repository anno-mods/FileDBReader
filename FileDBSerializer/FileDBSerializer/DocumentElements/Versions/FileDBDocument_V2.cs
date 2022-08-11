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
        public FileDBDocumentVersion VERSION { get; } = FileDBDocumentVersion.Version2;
        public byte[] MAGIC_BYTES { get => Versioning.GetMagicBytes(VERSION); }
        public int MAGIC_BYTE_COUNT { get => MAGIC_BYTES.Length; }
        public int OFFSET_TO_OFFSETS { get; } = 16;
        public ushort MAX_TAG_ID { get; set; } = 1; //0x01 0x00
        public ushort MAX_ATTRIB_ID { get; set; } = 32768; //0x01 0x80
        public static int ATTRIB_BLOCK_SIZE = 8;

        //CONSTRUCTORS
        public FileDBDocument_V2()
        {
            Roots = new List<FileDBNode>();
            Tags = new TagSection();
        }
    }
}
