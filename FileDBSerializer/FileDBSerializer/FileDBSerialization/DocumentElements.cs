using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices; 

namespace FileDBSerializing
{
    public class FileDBDocument
    {
        public List<FileDBNode> Roots = new List<FileDBNode>();
        public TagSection Tags = new TagSection();
        int AttribSectionOffset = 0;
        int TagSectionOffset = 0;
        byte[] MagicBytes = { 0x08, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0xFF, 0xFF };

        public static readonly int ATTRIB_BLOCK_SIZE = 8;
        public static readonly int OFFSET_TO_OFFSETS = 16;

        public static int GetBlockSpace(int bytesize)
        {
            return ((bytesize / FileDBDocument.ATTRIB_BLOCK_SIZE) * FileDBDocument.ATTRIB_BLOCK_SIZE + FileDBDocument.ATTRIB_BLOCK_SIZE * Math.Clamp(bytesize % FileDBDocument.ATTRIB_BLOCK_SIZE, 0, 1));
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
        public byte[] Content;
        public override String GetID()
        {
            if (ParentDoc.Tags.Attribs.TryGetValue((ushort)ID, out string value))
                return value;
            else return "a_" + ID; 
        }
    }
}
