using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileDBReader.src
{
    class FcFileHelper
    {
        /// <summary>
        /// CDATA to Hex conversion.
        /// 
        /// Anno: <Node>CDATA[<(Int32)Bytesize><BinaryData>]</Node>
        /// This: <Node>CDATA[Hex Representation of BinaryData]</Node>
        /// 
        /// Value conversion happens with the usual XmlInterpreter/XmlExporters. 
        /// </summary>

        public FcFileHelper()
        {

        }

        static readonly String CdataOpener = "CDATA[";

        public XmlDocument ReadFcFile(String Filename) 
        {
            return StreamToXmlFile(ReadCdataToHex(File.OpenRead(Filename)));
        }

        public XmlDocument StreamToXmlFile(Stream stream) {
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            return doc; 
        }

        public Stream ReadCdataToHex(FileStream fs) 
        {
            //initialize writer and reader
            var reader = new BinaryReader(fs);
            var Encoding = new UTF8Encoding();
            MemoryStream output = new MemoryStream();
            var writer = new BinaryWriter(output, Encoding);

            //initialize lastSixChars and Current Char
            String LastSixChars = "";
            char CurrentChar;

            //BinaryWriter writes length prefixed strings - this means we need to create a byte array from our encoding and let the BinaryWriter write that.
            writer.Write(Encoding.GetBytes("<Content>"));
            writer.Flush();
            //basically take the stream, find all cdata sections and write a new stream that is a valid xml document.
            while (fs.Position < fs.Length)
            {
                //advance stream position
                CurrentChar = reader.ReadChar();
                LastSixChars = Advance(LastSixChars, CurrentChar);

                //CDATA section found.
                if (LastSixChars.Equals(CdataOpener))
                {
                    //go to the beginning of the cdata section,
                    int bytesize = reader.ReadInt32();
                    var ContentSpan = new ReadOnlySpan<byte>(reader.ReadBytes(bytesize));

                    //reset output stream position to before CDATA, then write the beginning wellformed, then write the binary data as hex string
                    writer.BaseStream.Position -= 5;
                    writer.Flush();
                    writer.Write(Encoding.GetBytes("CDATA["));
                    writer.Write(Encoding.GetBytes(ContentSpan.ToHexString()));
                }
                else
                {
                    writer.Write(CurrentChar);
                    writer.Flush();
                }
            }
            writer.Write(Encoding.GetBytes("</Content>"));
            writer.Flush();
            //reset position before returning the stream.
            output.Position = 0;
            return output; 
        }

        public void WriteHexToCdata()
        { 
        
        }

        //deletes the first character from input, appends c to the end
        private String Advance(String input, char c)
        {
            if (input.Length >= 6)
            {
                input = input.Substring(1);
            }
            input += c;
            return input;
        }
    }
}
