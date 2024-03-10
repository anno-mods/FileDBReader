using System;
using System.Xml;
using System.IO;
using AnnoMods.BBDom;
using AnnoMods.BBDom.IO;
using AnnoMods.BBDom.XML;

namespace FileDBReader.src.XmlRepresentation
{
    public class Reader
    {
        public Reader()
        { 
        
        }

        public XmlDocument Read(Stream input)
        {
            //first of all, get the document version
            BBDocumentVersion FileVersion = VersionDetector.GetCompressionVersion(input);
            Console.WriteLine("[READER]: Autodetected FileVersion = {0}", FileVersion);
            input.Position = 0;

            var bbdoc = BBDocument.LoadStream(input);
            return bbdoc.ToXmlDocument();

            Console.WriteLine("[READER]: Only Version 1 and 2 are supported");
            throw new InvalidFileDBException();
        }
    }
}
