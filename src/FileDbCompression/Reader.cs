using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FileDBSerializing;
using FileDBReader.src;
using System.IO;
using FileDBReader.src.XmlSerialization;

namespace FileDBReader.src
{
    public class Reader
    {
        FileDbXmlSerializer FileDbToXml = new FileDbXmlSerializer();

        public Reader()
        { 
        
        }

        public XmlDocument Read(Stream input)
        {
            //first of all, get the document version
            int FileVersion = VersionDetector.GetCompressionVersion(input);
            Console.WriteLine("Autodetected FileVersion = {0}", FileVersion);
            input.Position = 0;
            try
            {
                if (FileVersion == 1)
                {
                    return Read_Version1(input);
                }
                else if (FileVersion == 2)
                {
                    return Read_Version2(input);
                }
            }
            catch (InvalidFileDBException)
            {
                Console.WriteLine("ERROR: Invalid FileDB file detected. Conversion was terminated");
            }
            Console.WriteLine("ERROR: Only Version 1 and 2 are supported");
            throw new InvalidFileDBException();
        }

        private XmlDocument Read_Version2(Stream input)
        {
            var deserializer = new FileDBDeserializer<FileDBDocument_V2>();
            FileDBDocument filedb = deserializer.Deserialize(input);
            return FileDbToXml.ToXml(filedb);
        }

        private XmlDocument Read_Version1(Stream input)
        {
            var deserializer = new FileDBDeserializer<FileDBDocument_V1>();
            FileDBDocument filedb = deserializer.Deserialize(input);
            return FileDbToXml.ToXml(filedb);
        }
    }
}
