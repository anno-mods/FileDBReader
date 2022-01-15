using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    internal interface IFileDBParser
    {
        /// <summary>
        /// Reads the Next Nodes ID and bytesize from the Reader and advances its position.
        /// </summary>
        /// <param name="bytesize">out bytesize of the next element: in case of tags and terminators, this is 0</param>
        /// <param name="id">out id of the next element.</param>
        /// <returns>An element of the States enum that describes what the next element will be logically.
        /// </returns>
        /// 
        States ReadNextOperation(out int _bytesize, out ushort _id);

        byte[] ReadAttribContent(int bytesize);

        TagSection ReadTagSection(int OffsetToOffsets);
        Dictionary<ushort, String> ParseDictionary(int Offset);

        Tag ReadTag(int ID, Tag ParentTag);
        Attrib ReadAttrib(int bytesize, int ID);
    }

    internal static class IFileDBParserExtensions
    {
        internal static Attrib CreateAttrib(this IFileDBParser parser, int bytesize, int ID, IFileDBDocument parent)
        {
            byte[] Content;
            Content = parser.ReadAttribContent(bytesize);
            return new Attrib() { ID = ID, Bytesize = bytesize, Content = Content, ParentDoc = parent};
        }

        internal static States DetermineState(this IFileDBParser parser, int ID)
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
