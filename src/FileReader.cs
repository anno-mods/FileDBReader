using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Syroot.BinaryData.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;

namespace FileDBReader
{

    /// <summary>
    /// FileReader decompressing base version done by VeraAtVersus
    /// 
    /// Converts a FileDB Compressed file to an xml representation with data represented in hex strings.
    /// </summary>
    public class FileReader
    {
        #region Properties

        #endregion Properties

        #region TopLevelMethods
        public XmlDocument ReadSpan(Span<byte> SpanToRead, int FileVersion)
        {
            //Init
            var document = new XDocument();
            var root = new XElement("Content");
            document.Add(root);
            XElement currentNode = null;

            var Filereader = new SpanReader(SpanToRead);
            //Set Position from The Tags Section

            int TagsOff = 0;
            switch (FileVersion)
            {
                //FileVersion 1
                case 1:
                    TagsOff = Filereader.Position = Convert.ToInt32(MemoryMarshal.Read<UInt32>(Filereader.Span[^4..]));
                    var Tags = ReadTagsSection_FileVersion1(ref Filereader, TagsOff);
                    var nodesReader = Filereader[..TagsOff];
                    return ReadNodeSection_FileVersion1(ref nodesReader, ref currentNode, ref document, Tags);
                //FileVersion 2
                case 2:
                    TagsOff = Filereader.Position = Convert.ToInt32(MemoryMarshal.Read<UInt32>(Filereader.Span[^16..]));
                    int AttribsOff = Filereader.Position = Convert.ToInt32(MemoryMarshal.Read<UInt32>(Filereader.Span[^12..]));
                    var TagsV2 = ReadTagsSection_FileVersion2(ref Filereader, TagsOff, AttribsOff);
                    var nodesReaderV2 = Filereader[..TagsOff];
                    return ReadNodeSection_FileVersion2(ref nodesReaderV2, ref currentNode, ref document, TagsV2);
                default:
                    throw new ArgumentException(String.Format("Invalid Compression Version! Only 1 and 2 are supported"));
            }
        }

        public XmlDocument ReadFile(string path, int FileVersion)
        {
            Span<byte> Span = File.ReadAllBytes(path).AsSpan();
            var document = ReadSpan(Span, FileVersion);
            return document;
        }

        private XmlDocument ToXmlDocument(XDocument doc)
        {
            var xml = new XmlDocument();
            using (var xmlReader = doc.CreateReader())
            {
                xml.Load(xmlReader);
                return xml;
            }
        }

        private static void AddToCurrentNode(XDocument document, XElement currentNode, XElement node)
        {
            //if nothing 
            if (currentNode == null)
            {
                //var newnode = new XElement("Base");
                //document.Root.Add(newnode);
                currentNode = document.Root;
            }
            currentNode.Add(node);
        }

        #endregion Methods

        #region FileVersion 1
        /// <summary>
        /// Reads the node section of File Version 1
        /// </summary>
        /// <param name="nodesReader"></param>
        /// <param name="currentNode"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        private XmlDocument ReadNodeSection_FileVersion1(ref SpanReader nodesReader, ref XElement currentNode, ref XDocument document, Dictionary<ushort, string> Tags)
        {
            while (nodesReader.Position < nodesReader.Length)
            {
                //Get Next ID
                nodesReader.ReadUInt16(out var nextId);

                //Close Node
                if (nextId == 0)
                {
                    currentNode = currentNode?.Parent == document.Root ? null : currentNode?.Parent;
                }
                //Check for Existing Id
                else if (Tags.TryGetValue(nextId, out var tag))
                {
                    //Tag
                    if (nextId < 32768)
                    {
                        var node = new XElement(tag);
                        AddToCurrentNode(document, currentNode, node);
                        currentNode = node;
                    }

                    //Attribute
                    else
                    {
                        nodesReader.ReadInt32Bit7(out var length);
                        var attribute = new XElement(tag);

                        //interpretation happens laterz
                        object content;
                        {
                            content = nodesReader.Span.Slice(nodesReader.Position, length).ToHexString();
                            nodesReader.Position += length;
                        }

                        attribute = new XElement(tag, content);
                        AddToCurrentNode(document, currentNode, attribute);
                    }
                }
            }
            return ToXmlDocument(document);
        }

        private static Dictionary<ushort, string> ReadTagsSection_FileVersion1(ref SpanReader reader, int TagSectionOffset)
        {
            reader.Position = TagSectionOffset;
            Dictionary<ushort, string> dictionary = new Dictionary<ushort, string>();

            dictionary.Add(1, "None");
            dictionary.Add(32768, "None");

            var TagCount = reader.ReadInt32Bit7();
            for (var i = 0; i < TagCount; i++)
            {
                reader.ReadString0(out var name);
                reader.ReadUInt16(out var id);

                //Xml Space in Name Fix
                //dictionary.Add(id, name.Replace(" ", "_"));

                //This upper fix will break recompression. we will have to think of something better in the future. :/
                dictionary.Add(id, name);
            }

            var AttribCount = reader.ReadInt32Bit7();
            for (var i = 0; i < AttribCount; i++)
            {
                reader.ReadString0(out var name);
                reader.ReadUInt16(out var id);
                dictionary.Add(id, name);

                //see above
            }

            return dictionary;
        }
        #endregion

        #region FileVersion 2

        /*
         * FileVersion 2: 
         * 
         * Tag and Attrib IDs are Int32s in the nodes section, but Int16's in the tags section. Probably this will be changed in a future file version. 
         * 
         * Tags and Attribs have their bytesize set before their ID: <bytesize> <id>
         * in case of tags, this bytesize is zero. The tag end also has a bytesize of 0. 
         * 
         * Attrib data is stored in 8 byte data blocks. if the bytesize doesn't match this, 0x00's are appended at the end until a full 8 bytes are reached.
         * Those additional bytes do not count towards the bytesize.
         * 
         * File Version 2 also has an eight byte footer: 0x08 0x00 0x00 0x00 0xFE 0xFF 0xFF 0xFF or (Int32) 8 -2
         */
        private XmlDocument ReadNodeSection_FileVersion2(ref SpanReader nodesReader, ref XElement currentNode, ref XDocument document, Dictionary<int, string> Tags)
        {
            while (nodesReader.Position < nodesReader.Length)
            {

                //Get Next ID
                nodesReader.ReadInt32(out var nextId);
                nodesReader.ReadInt32(out var NextPlusOneId);

                //Next ID is 0 AND the id after that is also 0: Close Node and go to nodes parent.
                if (nextId == 0 && NextPlusOneId == 0)
                {
                    currentNode = currentNode?.Parent == document.Root ? null : currentNode?.Parent;
                }
                //Check for Existing Id


                //Next ID is not 0
                //USHORT TYPECAST RIGHT HERE IS NOT NICE. THIS LIMITS INT32 TO INT16. WE NEED A SEPERATE DICTIONARY 
                else if (Tags.TryGetValue((ushort)NextPlusOneId, out var tag))
                {
                    //Tag
                    if (nextId == 0 && NextPlusOneId < 32768)
                    {
                        var node = new XElement(tag);
                        AddToCurrentNode(document, currentNode, node);
                        currentNode = node;
                    }

                    //Attribute
                    else
                    {
                        int bytesize = nextId;
                        var attribute = new XElement(tag);

                        object content;
                        {
                            content = nodesReader.Span.Slice(nodesReader.Position, bytesize).ToHexString();
                            nodesReader.Position += bytesize;
                        }

                        attribute = new XElement(tag, content);
                        AddToCurrentNode(document, currentNode, attribute);

                        //File Version 2 stores attribute content data in 64 bit blocks. if the bytesize does not match this, zeroes are appended. 
                        if (bytesize % 8 != 0)
                        {
                            int bytesToAdd = (8 - (bytesize % 8));
                            for (var i = 0; i < bytesToAdd; i++)
                            {
                                nodesReader.ReadByte();
                            }
                        }

                    }
                }
            }

            return ToXmlDocument(document);
        }


        /*  New Tag Section In File Version 2
         *  <Int32 TagCount> 
            per Tag: <Byte ID> <0x00>
            per Tag: <String Name> separated by <0x00>
            <0x00 00 00 00>

            <Int32 AttribCount>
            per Attrib: <Byte ID> <0x80>
            per Attrib: <String Name> separated by <0x00>
            <0x00 00 00 00>

            <0x00>

            <Int32 Tags Offset>
            <Int32 Attrib Offset>
            0x08 00 00 00 (8 als Int32)
            0xFE FF FF FF (-2 als Int32)
         */

        private static Dictionary<int, String> ReadTagsSection_FileVersion2(ref SpanReader reader, int TagSectionOffset, int AttribSectionOffset)
        {
            var Tags = ReadDictionary_FileVersion2(ref reader, TagSectionOffset);
            var Attribs = ReadDictionary_FileVersion2(ref reader, AttribSectionOffset);
            var result = Tags.Concat(Attribs).ToDictionary(x => x.Key, y => y.Value);

            result.Add(1, "None");
            result.Add(32768, "None");

            return result;
        }

        private static Dictionary<int, string> ReadDictionary_FileVersion2(ref SpanReader reader, int SectionOffset)
        {
            reader.Position = SectionOffset;
            Dictionary<int, string> dictionary = new Dictionary<int, string>();

            //add empty tag and attribute
            var count = reader.ReadInt32();

            List<int> IDs = new List<int>();

            for (var i = 0; i < count; i++)
            {
                reader.ReadUInt16(out var id);
                IDs.Add((int)id);
            }
            for (var i = 0; i < count; i++)
            {
                reader.ReadString0(out var name);
                dictionary.Add(IDs[i], name);

                //Xml Space in Name Fix
                //dictionary.Add(id, name.Replace(" ", "_"));
                //This upper fix will break recompression. we will have to think of something better in the future. :/
            }

            return dictionary;
        }
        #endregion
    }
}