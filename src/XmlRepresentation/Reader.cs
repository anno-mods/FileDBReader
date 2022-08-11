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

            var parser = new DocumentParser(FileVersion);
            IFileDBDocument filedb = parser.LoadFileDBDocument(input);
            return FileDbToXml.ToXml(filedb);

            Console.WriteLine("[READER]: Only Version 1 and 2 are supported");
            throw new InvalidFileDBException();
        }
    }
}
