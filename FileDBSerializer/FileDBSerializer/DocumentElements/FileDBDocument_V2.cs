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
        public int VERSION { get; }
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
            VERSION = 2;
        }

        //STATIC FUNCTIONS
        public static int GetBlockSpace(int bytesize)
        {
            return ((bytesize / FileDBDocument_V2.ATTRIB_BLOCK_SIZE) * FileDBDocument_V2.ATTRIB_BLOCK_SIZE + FileDBDocument_V2.ATTRIB_BLOCK_SIZE * Math.Clamp(bytesize % FileDBDocument_V2.ATTRIB_BLOCK_SIZE, 0, 1));
        }

        public static byte[] GetBytesInBlocks(byte[] attrib, int bytesize)
        {
            int ContentSize = GetBlockSpace(bytesize);
            Array.Resize<byte>(ref attrib, ContentSize);
            return attrib;
        }
        public static Int64 GetNodeTerminator()
        {
            return (Int64)0;
        }

        public Tag AddTag(String Name)
        {
            //default -> none id
            ushort IDOfThisTag = 0;
            //if the tags don't contain this value, we need to add it to the tag section. also, filter out the None tag name.
            if (!Tags.Tags.ContainsValue(Name) && !Name.Equals("None"))
            {
                MAX_TAG_ID++;
                IDOfThisTag = MAX_TAG_ID;
                Tags.Tags.Add(IDOfThisTag, Name);
            }
            else if (Tags.Tags.ContainsValue(Name))
            {
                IDOfThisTag = Tags.Tags.FirstOrDefault(x => x.Value == Name).Key;
            }

            return new Tag() { ID = IDOfThisTag, ParentDoc = this };
        }
        public Attrib AddAttrib(String Name)
        {
            //default -> none id
            ushort IDOfThisTag = 0;
            //if the tags don't contain this value, we need to add it to the tag section. also, filter out the None tag name.
            if (!Tags.Attribs.ContainsValue(Name) && !Name.Equals("None"))
            {
                MAX_ATTRIB_ID++;
                IDOfThisTag = MAX_ATTRIB_ID;
                Tags.Attribs.Add(IDOfThisTag, Name);
            }
            else if (Tags.Attribs.ContainsValue(Name))
            {
                IDOfThisTag = Tags.Attribs.FirstOrDefault(x => x.Value == Name).Key;
            }
            return new Attrib() { ID = IDOfThisTag, ParentDoc = this };
        }
        public IEnumerable<FileDBNode> SelectNodes(String Lookup)
        {
            return Roots.SelectNodes(Lookup);
        }

        public FileDBNode SelectSingleNode(String Lookup)
        {
            return Roots.SelectSingleNode(Lookup);
        }
    }
}
