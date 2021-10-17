using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using FileDBReader.src;

namespace FileDBReader
{

    /// <summary>
    /// Converts an xml file with data represented in hex strings into filedb compression readable by Anno 1800. 
    /// </summary>
    class FileWriter
    {
        public FileWriter() {

        }

        public Stream Export(String path, String outputFileFormat, int FileVersion)
        {
            var stream = File.Create(Path.ChangeExtension(path, outputFileFormat));
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            return Export(doc, stream, FileVersion);
        }

        public Stream Export(XmlDocument doc, String outputFileFormat, String path, int FileVersion)
        {
            var stream = File.Create(Path.ChangeExtension(path, outputFileFormat));
            return Export(doc, stream, FileVersion);
        }

        public Stream Export(XmlDocument doc, String OutputFile, int FileVersion)
        {
            var stream = File.Create(OutputFile);
            return Export(doc, stream, FileVersion);
        }

        //converts an xmlNode to fileDB Compression and returns the result as stream
        public Stream Export(XmlDocument xml, Stream stream, int FileVersion) {

            Dictionary<String, byte> Tags = new Dictionary<string, byte>();
            Dictionary<String, byte> Attribs = new Dictionary<string, byte>();

            Tags.Add("None", 1);
            //respect None Attrib that can happen from time to time
            Attribs.Add("None", 0);

            byte tagcount = 1;
            byte attribcount = 0;

            XmlNodeList nodes = xml.FirstChild.ChildNodes;

            BinaryWriter writer = new BinaryWriter(stream);
            foreach (XmlElement element in nodes)
            {
                switch (FileVersion)
                {
                    case 1:
                        writeNode_FileVersion1(element, ref Tags, ref Attribs, ref tagcount, ref attribcount, ref writer);
                        break;
                    case 2:
                        writeNode_FileVersion2(element, ref Tags, ref Attribs, ref tagcount, ref attribcount, ref writer);
                        break;
                }
            }

            //nullterminate data section
            Int16 nullchar = 0;
            writer.Write(nullchar);
            writer.Flush();

            //Replace Values in Tags and Attribs if we need to
            if (InvalidTagNameHelper.ReplaceOperations != null)
            {
                //second arg bool reverse needs to be true as we want to do the reverse operation of the initial replacement.
                Tags = InvalidTagNameHelper.RenameKeysInDictionary<byte>(Tags, true);
                Attribs = InvalidTagNameHelper.RenameKeysInDictionary<byte>(Attribs, true);
            }

            switch (FileVersion)
            {
                case 1:
                    writeTagSection_FileVersion1(ref Tags, ref Attribs, 0, ref writer);
                    break;
                case 2:
                    //before the tag section, fill the other section to a multiple of 8 bytes.
                    var streamSize = (int)writer.BaseStream.Position;
                    if (streamSize % 8 != 0)
                    {
                        int bytesToAdd = (8 - (streamSize % 8));
                        for (var i = 0; i < bytesToAdd; i++)
                        {
                            writer.Write((byte)0);
                        }
                    }
                    //write the text section
                    writeTagSection_FileVersion2(ref Tags, ref Attribs, 0, ref writer);
                    break;
            }

            return stream;
        }


        #region FileVersion1

        /// <summary>
        /// exports an xmlfile to filedb compression
        /// </summary>
        /// <param name="path"></param>
        /// <param name="outputFileFormat"></param>
        //pass by reference to increment original values
        public void writeNode_FileVersion1(XmlNode e, ref Dictionary<String, byte> Tags, ref Dictionary<String, byte> Attribs, ref byte tagcount, ref byte attribcount, ref BinaryWriter writer)
        {
            //if this does not contain text
            //is a tag
            var FirstChild = e.FirstChild;
            if (!(FirstChild != null && FirstChild.NodeType == XmlNodeType.Text))
            {
                //if key doesn't exist, add it
                if (!Tags.ContainsKey(e.Name))
                {
                    Tags.Add(e.Name, (byte)(tagcount + 1));
                    tagcount++;
                }

                //because we made a precheck this HAS to exist, no more checks needed.
                var id = Tags[e.Name];
                //write id
                //write 00
                writer.Write(id);
                writer.Write((byte)0);

                //write childnodes
                foreach (XmlNode element in e.ChildNodes)
                {
                    writeNode_FileVersion1(element, ref Tags, ref Attribs, ref tagcount, ref attribcount, ref writer);
                }
                //nullterminate tag
                Int16 nullchar = 0;
                writer.Write(nullchar);
                writer.Flush();
            }
            //else
            //e is an attribute
            else
            {
                //if key doesn't exist, add it
                if (!Attribs.ContainsKey(e.Name))
                {
                    Attribs.Add(e.Name, (byte)(attribcount + 1));
                    attribcount++;
                }

                var id = Attribs[e.Name];
                //write id
                //write 80 
                writer.Write(id);
                writer.Write((byte)128);

                //write bytesize of content 
                //write content 
                var bytes = HexHelper.StringToByteArray(e.InnerText);

                //write as 7 bit encoded integer - copied from BinaryReader where it is a protected method
                uint v = (uint)bytes.Length;   // support negative numbers
                while (v >= 0x80)
                {
                    writer.Write((byte)(v | 0x80));
                    v >>= 7;
                }
                writer.Write((byte)v);

                writer.Write(bytes);
                writer.Flush();
            }
        }

        public void writeTagSection_FileVersion1(ref Dictionary<String, byte> Tags, ref Dictionary<String, byte> Attribs, int offset, ref BinaryWriter writer) {
            var TagSectionOffset = (int)writer.BaseStream.Position;

            //Tags
            writeDictionary_FileVersion1(ref Tags, offset, ref writer, (byte)0);

            //Attribs
            writeDictionary_FileVersion1(ref Attribs, offset, ref writer, (byte)128);

            //Bytesize offset
            writer.Write(TagSectionOffset);
            writer.Flush();
        }

        public void writeDictionary_FileVersion1(ref Dictionary<String, byte> Tags,int offset, ref BinaryWriter writer, byte IDseparator)
        {
            Tags.Remove("None");
            writer.Write((byte)(Tags.Count));

            foreach (String s in Tags.Keys)
            {
                var TagName = s;
                writer.Write(Encoding.UTF8.GetBytes(TagName));
                byte i = Tags[s];
                writer.Write((byte)0);
                writer.Write(i);
                writer.Write(IDseparator);
            }
            writer.Flush();
        }

        #endregion

        #region FileVersion2

        //pass by reference to increment original values
        public void writeNode_FileVersion2(XmlNode e, ref Dictionary<String, byte> Tags, ref Dictionary<String, byte> Attribs, ref byte tagcount, ref byte attribcount, ref BinaryWriter writer)
        {
            //if this does not contain text
            //is a tag
            var FirstChild = e.FirstChild;
            if (!(FirstChild != null && FirstChild.NodeType == XmlNodeType.Text))
            {
                //if key doesn't exist, add it
                if (!Tags.ContainsKey(e.Name))
                {
                    Tags.Add(e.Name, (byte)(tagcount + 1));
                    tagcount++;
                }


                //write Int32 Bytesize (in case of tags: 0)
                //write Int32 ID
                writer.Write(0);

                //because we made a precheck this HAS to exist, no more checks needed.
                var id = Tags[e.Name];
                //write the id
                writer.Write((Int32)id);

                //write childnodes
                foreach (XmlNode element in e.ChildNodes)
                {
                    writeNode_FileVersion2(element, ref Tags, ref Attribs, ref tagcount, ref attribcount, ref writer);
                }

                //nullterminate tag - 4 bytes bytesize, 4 bytes ID: 0, is equal to 8 bytes 0.
                Int64 nullchar = 0;
                writer.Write(nullchar);
                writer.Flush();
            }
            //else
            //e is an attribute
            else
            {
                //if key doesn't exist, add it
                if (!Attribs.ContainsKey(e.Name))
                {
                    Attribs.Add(e.Name, (byte)(attribcount + 1));
                    attribcount++;
                }

                //write bytesize of content 
                //write attrib id
                //write content 
                var bytes = HexHelper.StringToByteArray(e.InnerText);

                int bytesize = bytes.Length;
                writer.Write(bytesize);

                var id = Attribs[e.Name];
                //write id
                //write 80 
                //fill to four bytes
                writer.Write(id);
                writer.Write((byte)128);
                writer.Write((short)0);

                writer.Write(bytes);

                //fill bytes to get full 8 byte blocks.
                if (bytesize % 8 != 0)
                {
                    int bytesToAdd = (8 - (bytesize % 8));
                    for (var i = 0; i < bytesToAdd; i++)
                    {
                        writer.Write((byte)0);
                    }
                }

                writer.Flush();
            }
        }

        public void writeTagSection_FileVersion2(ref Dictionary<String, byte> Tags, ref Dictionary<String, byte> Attribs, int offset, ref BinaryWriter writer)
        {
            var TagSectionOffset = (int)writer.BaseStream.Position;

            writeDictionary_FileVersion2(Tags, ref writer, (byte)0);

            //three bytes to fill at the end
            for (int i = 0; i < 3; i++) {
                writer.Write((byte)0);
            }

            var AttribSectionOffset = (int)writer.BaseStream.Position;

            writeDictionary_FileVersion2(Attribs, ref writer, (byte)128);
            writer.Write((Int32)0);

            //Bytesize offset
            writer.Write(TagSectionOffset);
            writer.Write(AttribSectionOffset);

            writer.Write((Int32)8);
            writer.Write((Int32)(-2));
            writer.Flush();
        }

        public void writeDictionary_FileVersion2(Dictionary<String, byte> Tags, ref BinaryWriter writer, byte separator)
        {
            Tags.Remove("None");
            writer.Write((Int32)(Tags.Count));

            //write ids
            foreach (string s in Tags.Keys)
            {
                byte i = Tags[s];
                writer.Write(i);
                writer.Write(separator);
            }
            //write names divided with zeroes
            foreach (string s in Tags.Keys)
            {
                var TagName = s; 
                writer.Write(Encoding.UTF8.GetBytes(TagName));
                writer.Write((byte)0);
            }
            writer.Flush();
        }

        #endregion



    }
    
}
