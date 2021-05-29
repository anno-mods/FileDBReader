using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;



namespace FileDBReader
{


    /// <summary>
    /// Converts an xml file with data represented in hex strings into filedb compression readable by Anno 1800. 
    /// </summary>
    class FileWriter
    {
        public FileWriter() { 
        
        }

        //converts an xmlNode to fileDB Compression and returns the result as byte span
        public Stream Export(XmlDocument xml, Stream stream) {

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
                writeNode(element, ref Tags, ref Attribs, ref tagcount, ref attribcount, ref writer);
            }

            //nullterminate data section
            Int16 nullchar = 0;
            writer.Write(nullchar);
            writer.Flush();

            writeTagSection(ref Tags, ref Attribs, 0, ref writer);
            return stream;
        }


        /// <summary>
        /// exports an xmlfile to filedb compression
        /// </summary>
        /// <param name="path"></param>
        /// <param name="outputFileFormat"></param>
        public void Export(String path, String outputFileFormat)
        {
            var stream = File.Create(Path.ChangeExtension(path, outputFileFormat));
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            Export(doc, stream);
        }

        public void Export(XmlDocument doc, String outputFileFormat, String path)
        {
            var stream = File.Create(Path.ChangeExtension(path, outputFileFormat));
            Export(doc, stream);
        }

        public void Export(XmlDocument doc, String OutputFile)
        {
            var stream = File.Create(OutputFile);
            Export(doc, stream);
        }

        //pass by reference to increment original values
        public void writeNode(XmlNode e, ref Dictionary<String, byte> Tags, ref Dictionary<String, byte> Attribs, ref byte tagcount, ref byte attribcount, ref BinaryWriter writer) 
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
                byte b = 0;
                writer.Write(b);


                //write childnodes
                foreach (XmlNode element in e.ChildNodes)
                {
                    writeNode(element, ref Tags, ref Attribs, ref tagcount, ref attribcount, ref writer);
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
                byte b = 128;
                writer.Write(b);

                //write bytesize of content 
                //write content 
                var bytes = StringToByteArray(e.InnerText);

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

        public void writeTagSection(ref Dictionary <String, byte> Tags, ref Dictionary<String, byte> Attribs, int offset, ref BinaryWriter writer) {
            //write tag count as 8 bit int
            //foreach tag in dictionary
            //write tag as nullterminated string 
            //write tag id as 8 bit int
            //write 00



            //write attrib count as 8 bit int
            //foreach attrib in dictionary
            //write attrib id as nullterminated string
            //write attrib id as 8 bit int 
            //write 80 

            //write offset

            var TagSectionOffset = (int)writer.BaseStream.Position;

            //Tags
            //make sure the empty none tag is rekt
            writer.Write((byte)(Tags.Count -1));
            foreach (String s in Tags.Keys) {
                //WE DO NOT WANT STRING LENGTH THANKS
                if (!s.Equals("None")) {
                    writer.Write(Encoding.UTF8.GetBytes(s));
                    byte i = Tags[s];
                    writer.Write((byte)0);
                    writer.Write(i);
                    writer.Write((byte)0);
                }
            }
            writer.Flush();

            //Attribs
            writer.Write((byte)Attribs.Count);
            foreach (String s in Attribs.Keys)
            {
                //WE DO NOT WANT STRING LENGTH THANKS
                if (!s.Equals("None")) {
                    writer.Write(Encoding.UTF8.GetBytes(s));
                    byte i = Attribs[s];
                    writer.Write((byte)0);
                    writer.Write(i);
                    writer.Write((byte)128);
                }
                
            }
            writer.Flush();

            //Bytesize offset
            writer.Write(TagSectionOffset);
            writer.Flush();
        }

        //copied from https://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array/321404 because why not
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

    }
    
}
