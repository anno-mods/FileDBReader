using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FileDBSerializing
{

    public interface FileDBDocument
    {
        public List<FileDBNode> Roots { get; set; }
        public int ElementCount { get; }
        public TagSection Tags { get; set; }
        public static int OFFSET_TO_OFFSETS { get; set; }
    }
    [DebuggerDisplay("[FileDB_Document: Version = 1, Count = {ElementCount}]")]
    public class FileDBDocument_V1 : FileDBDocument
    { 
        //FIELDS
        public List<FileDBNode> Roots { get; set; }
        public TagSection Tags { get; set; }

        public int ElementCount
        {
            get
            {
                return Roots.Count;
            }
        }

        new public static int OFFSET_TO_OFFSETS = 4;

        //CONSTRUCTORS
        public FileDBDocument_V1()
        {
            Roots = new List<FileDBNode>();
            Tags = new TagSection();
        }
    }
    [DebuggerDisplay("[FileDB_Document: Version = 2, Count = {ElementCount}]")]
    public class FileDBDocument_V2 : FileDBDocument
    {
        //FIELDS 
        public List<FileDBNode> Roots { get; set; }
        public TagSection Tags { get; set;  }

        public int ElementCount
        {
            get
            {
                return Roots.Count;
            }
        }

        byte[] MagicBytes = { 0x08, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0xFF, 0xFF };

        new public static int OFFSET_TO_OFFSETS = 16;

        public static int ATTRIB_BLOCK_SIZE = 8;

        //CONSTRUCTORS
        public FileDBDocument_V2()
        {
            Roots = new List<FileDBNode>();
            Tags = new TagSection();
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
        public override String GetID()
        {
            if (ParentDoc.Tags.Attribs.TryGetValue((ushort)ID, out string value))
                return value;
            else return "a_" + ID; 
        }
    }
}
