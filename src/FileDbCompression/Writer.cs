using FileDBSerializing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using FileDBReader.src.XmlSerialization;

namespace FileDBReader.src
{
    public class Writer
    {
        FileDBSerializer FileDBSerializer = new FileDBSerializer(); 
        public Writer()
        { 
        
        }

        public Stream Write(XmlDocument doc, Stream Stream, int FileVersion)
        {
            FileDBDocument filedb;
            try
            {
                if (FileVersion == 1)
                {
                    XmlFileDbSerializer<FileDBDocument_V1> serializer = new XmlFileDbSerializer<FileDBDocument_V1>();
                    filedb = serializer.ToFileDb(doc);
                    return FileDBSerializer.Serialize(filedb, Stream);
                }
                else if (FileVersion == 2)
                {
                    XmlFileDbSerializer<FileDBDocument_V2> serializer = new XmlFileDbSerializer<FileDBDocument_V2>();
                    filedb = serializer.ToFileDb(doc);
                    return FileDBSerializer.Serialize(filedb, Stream);
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
