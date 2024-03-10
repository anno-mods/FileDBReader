using FileDBSerializing;
using System;
using System.IO;
using System.Xml;

namespace FileDBReader.src.XmlRepresentation
{
    public class Writer
    {
        public Writer()
        { 
        
        }

        public Stream Write(XmlDocument doc, Stream Stream, int FileVersion)
        {
            BBDocumentVersion? version = null;
            try
            {
                version = (BBDocumentVersion) FileVersion;

                var documentWriter = new BBDocumentWriter(version.Value);
                XmlFileDbConverter converter = new XmlFileDbConverter(version.Value);
                var filedb = converter.ToFileDb(doc);
                return documentWriter.WriteToStream(filedb, Stream);
            }
            catch (InvalidCastException ex)
            {
                throw new ArgumentException("[WRITER]: Unsupported File Version!");
            }
            catch (InvalidXmlDocumentInputException)
            {
                Console.WriteLine("[WRITER]: Invalid XML Document input");
            }
            return null;
        }

    }
}
