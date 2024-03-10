using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoMods.BBDom.IO
{
    public interface IBBStructureParser
{
    public BinaryReader Reader { get; }
    public BBDocument? TargetDocument { get; set; }

    /// <summary>
    /// Reads the Next Nodes ID and bytesize from the Reader and advances its position.
    /// </summary>
    /// <param name="bytesize">out bytesize of the next element: in case of tags and terminators, this is 0</param>
    /// <param name="id">out id of the next element.</param>
    /// <returns>An element of the States enum that describes what the next element will be logically.
    /// </returns>
    States ReadNextOperation(out int _bytesize, out ushort _id);

    /// <summary>
    /// Reads the content of an Attribute into a byte array
    /// </summary>
    /// <param name="bytesize">the amount of bytes to read</param>
    /// <returns></returns>
    byte[] ReadAttribContent(int bytesize);

    /// <summary>
    /// Parses the stream into a TagSection.
    /// </summary>
    /// <param name="OffsetToOffsets">the offset to the end of the stream that holds the dictionary offsets.</param>
    /// <returns>A Tag Section</returns>
    TagSection ReadTagSection(int OffsetToOffsets);

    /// <summary>
    /// Parses a dictionary that holds ID -> Name
    /// </summary>
    /// <param name="Offset">the Offset to the start of the parsing stream where the dictionary begins</param>
    /// <returns>the parsed dictionary</returns>
    Dictionary<ushort, string> ParseDictionary(int Offset);

    /// <summary>
    /// Constructs a new Tag from the given bytesize and ParentTag.
    /// </summary>
    /// <param name="ID">the ID of the tag</param>
    /// <param name="ParentTag">The parent Tag of the tag to be constructed</param>
    /// <returns>the constructed tag</returns>
    Tag ReadTag(int ID, Tag ParentTag);

    /// <summary>
    /// Constructs an Attrib with the provided node ID and bytesize.
    /// </summary>
    /// <param name="bytesize">the size of the attribute content</param>
    /// <param name="ID">the ID of the attrib to be constructed</param>
    /// <returns>the constructed attribute.</returns>
    Attrib ReadAttrib(int bytesize, int ID, Tag ParentTag);

    void RegisterDocument(BBDocument doc)
    {
        TargetDocument = doc;
    }
}

public static class IFileDBParserExtensions
{
    public static Attrib CreateAttrib(this IBBStructureParser parser, int bytesize, int ID, BBDocument parent)
    {
        byte[] Content;
        Content = parser.ReadAttribContent(bytesize);
        return new Attrib() { ID = ID, Bytesize = bytesize, Content = Content, ParentDoc = parent };
    }

    public static States DetermineState(this IBBStructureParser parser, int ID)
    {
        switch (ID)
        {
            //we found attrib
            case >= 32768:
                return States.Attrib;
            //we found tag
            case < 32768 when ID != 0:
                return States.Tag;
            //we found terminator
            case <= 0:
                return States.Terminator;
        }
        return States.Undefined;
    }
}
}
