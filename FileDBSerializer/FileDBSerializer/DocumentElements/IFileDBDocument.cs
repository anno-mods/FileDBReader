using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    public interface IFileDBDocument
    {
        public List<FileDBNode> Roots { get; set; }
        public TagSection Tags { get; set; }
        public int ELEMENT_COUNT { get; }
        public int VERSION { get; }
        public int OFFSET_TO_OFFSETS { get; }
        public byte[] MAGIC_BYTES { get; }
        public int MAGIC_BYTE_COUNT { get; }
        public ushort MAX_ATTRIB_ID { get; set; }
        public ushort MAX_TAG_ID { get; set; }

        public Tag AddTag(String NodeName);
        public Attrib AddAttrib(String NodeName);

        public IEnumerable<FileDBNode> SelectNodes(String Lookup);
        public FileDBNode SelectSingleNode(String Lookup);
    }
}
