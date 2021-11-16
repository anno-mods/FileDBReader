using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FileDBSerializing
{
    public enum FileDBNodeType { Tag, Attrib }
    public interface FileDBDocument
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

        public Tag AddTag(String Name);
        public Attrib AddAttrib(String Attrib);

        public IEnumerable<FileDBNode> SelectNodes(String Lookup);
    }

    [DebuggerDisplay("[FileDB_Document: Version = 1, Count = {ELEMENT_COUNT}]")]
    public class FileDBDocument_V1 : FileDBDocument
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
        public static Int16 GetNodeTerminator()
        {
            return (Int16)0;
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
    }
    [DebuggerDisplay("[FileDB_Document: Version = 2, Count = {ELEMENT_COUNT}]")]
    public class FileDBDocument_V2 : FileDBDocument
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

        public ushort MAX_ATTRIB_ID {get => _max_attrib_id; set => _max_attrib_id = value; }

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
    }

    public class TagSection
    {
        public Dictionary<ushort, String> Tags;
        public Dictionary<ushort, String> Attribs;

        public TagSection()
        {
            Tags = new Dictionary<ushort, String>();
            Attribs = new Dictionary<ushort, String>();
            Tags.Add(1, "None");
            Attribs.Add(32768, "None");
        }

        public TagSection(Dictionary<ushort, String> tags, Dictionary<ushort, String> attribs)
        {
            Tags = tags;
            Attribs = attribs;
            Tags.Add(1, "None");
            Attribs.Add(32768, "None");
        }
    }

    public abstract class FileDBNode
    {
        public FileDBDocument ParentDoc = null;
        public Tag Parent = null;
        public int Bytesize = 0;
        public int ID = -1;

        public FileDBNodeType NodeType; 
        public abstract String GetID();

        public abstract String GetName();
    }

    [DebuggerDisplay("[FileDB_Tag: ID = {ID}, ChildCount = {ChildCount}]")]
    public class Tag : FileDBNode
    {
        public List<FileDBNode> Children = new List<FileDBNode>();
        //we are only allowed to get bytesize because it is needed when writing. Otherwise, bytesize for tags is always 0!
        public int ChildCount
        {
            get
            {
                return Children.Count;
            }
        }

        new int Bytesize { get; }

        public Tag()
        {
            Bytesize = 0;
            NodeType = FileDBNodeType.Tag;
        }

        public void AddChild(FileDBNode node)
        {
            Children.Add(node);
        }

        public override String GetID()
        {
            if (ParentDoc.Tags.Tags.TryGetValue((ushort)ID, out string value))
                return value;
            else return "t_" + ID;
        }

        public override string GetName()
        {
            return ParentDoc.Tags.Tags[(ushort)ID];
        }
        public IEnumerable<FileDBNode> SelectNodes(String Lookup)
        {
            return Children.SelectNodes(Lookup);
        }
    }

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
