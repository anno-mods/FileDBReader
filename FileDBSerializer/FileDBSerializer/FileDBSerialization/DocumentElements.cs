using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
    }

    [DebuggerDisplay("[FileDB_Document: Version = 1, Count = {ElementCount}]")]
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

        //INTERNAL MEMBERS

        internal static int _offset_to_offsets = 4;

        internal static byte[] _magic_bytes = new byte[0]; 
        
        internal static Type ID_TYPE = typeof(ushort);

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
    }
    [DebuggerDisplay("[FileDB_Document: Version = 2, Count = {ElementCount}]")]
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

        //INTERNAL MEMBERS: Just for Binding the public ones.

        internal static int ATTRIB_BLOCK_SIZE = 8;

        internal static int _offset_to_offsets = 16;

        internal static byte[] _magic_bytes = { 0x08, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0xFF, 0xFF };

        internal static int _magic_byte_count = _magic_bytes.Length;

        internal static Type ID_TYPE = typeof(uint);

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
    }

    [DebuggerDisplay("[FileDB_Tag: ID = {ID}, ChildCount = {ChildCount}]")]
    public class Tag : FileDBNode
    {
        public List<FileDBNode> Children = new List<FileDBNode>();
        //we are only allowed to get bytesize because it is needed when writing. Otherwise, bytesize for tags is always 0!
        new public int Bytesize { get; }
        public int ChildCount
        {
            get
            {
                return Children.Count;
            }
        }

        public Tag()
        {
            Bytesize = 0;
            NodeType = FileDBNodeType.Tag;
        }

        public override String GetID()
        {
            if (ParentDoc.Tags.Tags.TryGetValue((ushort)ID, out string value))
                return value;
            else return "t_" + ID;
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

        public MemoryStream ContentToStream()
        {
            return new MemoryStream(Content);
        }
    }
}
