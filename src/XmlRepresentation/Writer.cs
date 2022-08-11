using FileDBSerializing;
using System;
using System.IO;
using System.Xml;

namespace FileDBReader.src.XmlRepresentation
{
    public class Writer
    {
        DocumentWriter DocumentWriter = new DocumentWriter(); 
        public Writer()
        { 
        
        }

        public Stream Write(XmlDocument doc, Stream Stream, int FileVersion)
        {
            FileDBDocumentVersion? version = null;
            try
            {
                version = (FileDBDocumentVersion) FileVersion;

                XmlFileDbConverter converter = new XmlFileDbConverter(version.Value);
                var filedb = converter.ToFileDb(doc);
                return DocumentWriter.WriteFileDBToStream(filedb, Stream);
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
