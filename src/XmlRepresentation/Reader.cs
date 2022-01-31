using System;
using System.Xml;
using FileDBSerializing;
using System.IO;

namespace FileDBReader.src.XmlRepresentation
{
    public class Reader
    {
        FileDbXmlConverter FileDbToXml = new FileDbXmlConverter();

        public Reader()
        { 
        
        }

        public XmlDocument Read(Stream input)
        {
            //first of all, get the document version
            FileDBDocumentVersion FileVersion = VersionDetector.GetCompressionVersion(input);
            Console.WriteLine("[READER]: Autodetected FileVersion = {0}", FileVersion);
            input.Position = 0;
            try
            {
                if (FileVersion == FileDBDocumentVersion.Version1)
                {
                    return Read_Version1(input);
                }
                else if (FileVersion == FileDBDocumentVersion.Version2)
                {
                    return Read_Version2(input);
                }
            }
            catch (InvalidFileDBException)
            {
                Console.WriteLine("[READER]: Invalid FileDB file detected. Conversion was terminated");
            }
            Console.WriteLine("[READER]: Only Version 1 and 2 are supported");
            throw new InvalidFileDBException();
        }

        private XmlDocument Read_Version2(Stream input)
        {
            var deserializer = new DocumentParser<FileDBDocument_V2>();
            IFileDBDocument filedb = deserializer.LoadFileDBDocument(input);
            return FileDbToXml.ToXml(filedb);
        }

        private XmlDocument Read_Version1(Stream input)
        {
            var deserializer = new DocumentParser<FileDBDocument_V1>();
            IFileDBDocument filedb = deserializer.LoadFileDBDocument(input);
            return FileDbToXml.ToXml(filedb);
        }
    }
}
