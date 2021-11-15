using FileDBReader.src;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileDBReader
{
    public enum ConversionMode { Write, Read }
    public class FcFileHelper
    {
        /// <summary>
        /// CDATA to Hex conversion.
        /// 
        /// Anno: <Node>CDATA[<(Int32)Bytesize><BinaryData>]</Node>
        /// This: <Node>CDATA[Hex Representation of BinaryData]</Node>
        /// 
        /// Value conversion happens with the usual XmlInterpreter/XmlExporters. 
        /// </summary>
        /// 
        readonly private static String CdataOpener = "CDATA[";

        public FcFileHelper()
        {

        }

        public XmlDocument ReadFcFile(String Filename)
        {
            return StreamToXmlFile(ConvertFile(File.OpenRead(Filename), ConversionMode.Read));
        }

        public XmlDocument StreamToXmlFile(Stream stream) {
            XmlDocument doc = new XmlDocument();
            stream.Position = 0; 
            doc.Load(stream);
            return doc;
        }

        public Stream XmlFileToStream(XmlDocument doc)
        {
            Stream stream = new MemoryStream();
            doc.Save(stream);
            stream.Flush();
            stream.Position = 0;
            return stream; 
        }

        public void SaveStreamToFile(Stream Stream, String Filename)
        {
            var fs = File.Create(Filename);
            Stream.Position = 0;
            Stream.CopyTo(fs);
            fs.Close();
        }

        public Stream ConvertFile(Stream fs, ConversionMode mode)
        {
            CorrectMultiRoots correctMultiRoots = new CorrectMultiRoots();
            if (mode == ConversionMode.Read)
            {
                fs = correctMultiRoots.AddMultiRoot(fs);
            }
            else if (mode == ConversionMode.Write)
            {
                fs = correctMultiRoots.RemoveMultiRoot(fs);
            }

            //initialize writer and reader
            var reader = new BinaryReader(fs);
            var Encoding = new UTF8Encoding();
            MemoryStream output = new MemoryStream();
            var writer = new BinaryWriter(output, Encoding);

            //initialize lastSixChars and Current Char
            String LastSixChars = "";
            char CurrentChar;

            fs.Position = 0;

            //basically take the stream, find all cdata sections and write a new stream that is a valid xml document.
            CorrectEmptyClosingTag correctClose = new CorrectEmptyClosingTag();
            while (fs.Position < fs.Length)
            {
                //advance stream position
                CurrentChar = reader.ReadChar();
                LastSixChars = Advance(LastSixChars, CurrentChar);
                correctClose.Advance(CurrentChar);

                //CDATA section found.
                if (LastSixChars.Equals(CdataOpener))
                {
                    if (mode == ConversionMode.Read)
                        BinaryToHex(ref reader, ref writer);
                    else if (mode == ConversionMode.Write)
                        HexToBinary(ref reader, ref writer);
                }
                // Files like .rdp use short close </> which are not valid XML.
                else if (correctClose.IsClosing() && mode == ConversionMode.Read)
                {
                    writer.Write(correctClose.GetCorrection());
                    writer.Write(CurrentChar);
                    writer.Flush();
                }
                else
                {
                    writer.Write(CurrentChar);
                    writer.Flush();
                }
            }

            return output;
        }

        private void HexToBinary(ref BinaryReader reader, ref BinaryWriter writer)
        {
            String s = "";
            //get hex string up to ] character.
            char CurrentChar; 
            while ((CurrentChar = reader.ReadChar()) != ']')
            {
                s += CurrentChar;
            }
            //turn hex string into byte array
            var bytes = HexHelper.BytesFromBinHex(s);
            //write the cdata section
            writer.Write('[');
            writer.Write(bytes.Length);
            writer.Write(bytes);
            writer.Write(']');
        }

        private void BinaryToHex(ref BinaryReader reader, ref BinaryWriter writer)
        {
            var Encoding = new UTF8Encoding();
            //go to the beginning of the cdata section,
            int bytesize = reader.ReadInt32();
            var ContentSpan = new ReadOnlySpan<byte>(reader.ReadBytes(bytesize));

            //reset output stream position to before CDATA, then write the beginning wellformed, then write the binary data as hex string
            writer.BaseStream.Position -= 5;
            writer.Flush();
            writer.Write(Encoding.GetBytes("CDATA["));
            writer.Write(Encoding.GetBytes(ContentSpan.ToHexString()));
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
