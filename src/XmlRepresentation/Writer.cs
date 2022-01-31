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
            IFileDBDocument filedb;
            try
            {
                if (FileVersion == 1)
                {
                    XmlFileDbConverter<FileDBDocument_V1> serializer = new XmlFileDbConverter<FileDBDocument_V1>();
                    filedb = serializer.ToFileDb(doc);
                    return DocumentWriter.WriteFileDBToStream(filedb, Stream);
                }
                else if (FileVersion == 2)
                {
                    XmlFileDbConverter<FileDBDocument_V2> serializer = new XmlFileDbConverter<FileDBDocument_V2>();
                    filedb = serializer.ToFileDb(doc);
                    return DocumentWriter.WriteFileDBToStream(filedb, Stream);
                }
                else throw new ArgumentException("[WRITER]: Supported FileVersions are 1 and 2!");
            }
            catch (InvalidXmlDocumentInputException) 
            {
                Console.WriteLine("[WRITER]: Invalid XML Document input");
            }

            return null;
        }

    }
}
