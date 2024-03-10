using System;
using System.Xml;
using FileDBSerializing;
using System.IO;
using FileDBSerializer;

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
            BBDocumentVersion FileVersion = VersionDetector.GetCompressionVersion(input);
            Console.WriteLine("[READER]: Autodetected FileVersion = {0}", FileVersion);
            input.Position = 0;

            var parser = new BBDocumentParser(FileVersion);
            BBDocument filedb = parser.LoadBBDocument(input);
            return FileDbToXml.ToXml(filedb);

            Console.WriteLine("[READER]: Only Version 1 and 2 are supported");
            throw new InvalidFileDBException();
        }
    }
}
