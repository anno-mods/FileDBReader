using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    public enum FileDBDocumentVersion { Unknown = -1, Version1 = 1, Version2 = 2, }

    public interface IFileDBDocument
    {
        public List<FileDBNode> Roots { get; set; }
        public TagSection Tags { get; set; }
        public int ELEMENT_COUNT { get; }
        public FileDBDocumentVersion VERSION { get; }
        public int OFFSET_TO_OFFSETS { get; }
        public byte[] MAGIC_BYTES { get; }
        public int MAGIC_BYTE_COUNT { get; }
        public ushort MAX_ATTRIB_ID { get; set; }
        public ushort MAX_TAG_ID { get; set; }
    }

    public static class IFileDBDocumentExtensions
    {
        public static Tag AddTag(this IFileDBDocument doc, String Name)
        {
            //default -> none id
            ushort IDOfThisTag = 0;
            //if the tags don't contain this value, we need to add it to the tag section. also, filter out the None tag name.
            if (!doc.Tags.Tags.ContainsValue(Name) && !Name.Equals("None"))
            {
                doc.MAX_TAG_ID++;
                IDOfThisTag = doc.MAX_TAG_ID;
                doc.Tags.Tags.Add(IDOfThisTag, Name);
            }
            else if (doc.Tags.Tags.ContainsValue(Name))
            {
                IDOfThisTag = doc.Tags.Tags.FirstOrDefault(x => x.Value == Name).Key;
            }

            return new Tag() { ID = IDOfThisTag, ParentDoc = doc };
        }

        public static Attrib AddAttrib(this IFileDBDocument doc, String Name)
        {
            //default -> none id
            ushort IDOfThisTag = 0;
            //if the tags don't contain this value, we need to add it to the tag section. also, filter out the None tag name.
            if (!doc.Tags.Attribs.ContainsValue(Name) && !Name.Equals("None"))
            {
                doc.MAX_ATTRIB_ID++;
                IDOfThisTag = doc.MAX_ATTRIB_ID;
                doc.Tags.Attribs.Add(IDOfThisTag, Name);
            }
            else if (doc.Tags.Attribs.ContainsValue(Name))
            {
                IDOfThisTag = doc.Tags.Attribs.FirstOrDefault(x => x.Value == Name).Key;
            }
            return new Attrib() { ID = IDOfThisTag, ParentDoc = doc };
        }
    }
}
