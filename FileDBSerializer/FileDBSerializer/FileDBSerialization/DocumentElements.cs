using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices; 

namespace FileDBSerializing
{
    public interface FileDBDocument
    {
        public List<FileDBNode> Roots { get; set; }
        public TagSection Tags { get; set; }
        public static int OFFSET_TO_OFFSETS { get; set; }
    }

    public class FileDBDocument_V1 : FileDBDocument
    { 
        public List<FileDBNode> Roots { get; set; }
        public TagSection Tags { get; set; }
        public FileDBDocument_V1()
        {
            Roots = new List<FileDBNode>();
            Tags = new TagSection(); 
        }

        new public static int OFFSET_TO_OFFSETS = 4;
    }

    public class FileDBDocument_V2 : FileDBDocument
    {
        public List<FileDBNode> Roots { get; set; }
        public TagSection Tags { get; set;  }
        public FileDBDocument_V2()
        {
            Roots = new List<FileDBNode>();
            Tags = new TagSection();
        }

        byte[] MagicBytes = { 0x08, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0xFF, 0xFF };

        new public static int OFFSET_TO_OFFSETS = 16;

        public static int ATTRIB_BLOCK_SIZE = 8;

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

    abstract public class FileDBNode
    {
        public FileDBDocument ParentDoc = null;
        public Tag Parent = null;
        public int Bytesize = 0;
        public int ID = 0;

        public abstract String GetID();
    }

    public class Tag : FileDBNode
    {
        public List<FileDBNode> Children = new List<FileDBNode>();

        public override String GetID()
        {
            if (ParentDoc.Tags.Tags.TryGetValue((ushort)ID, out string value))
                return value;
            else return "t_" + ID;
        }
    }

    public class Attrib : FileDBNode
    {
        public byte[] Content
        {
            get { return Content; }
            set 
            { 
                Content = value;
                Bytesize = Content.Length;
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
