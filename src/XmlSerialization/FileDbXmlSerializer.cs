using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using FileDBSerializing;

namespace FileDBReader.src.XmlSerialization
{
    class FileDbXmlSerializer
    {
        XmlDocument doc; 
        public XmlDocument ToXml(FileDBDocument filedb)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            doc = new XmlDocument();

            var Root = doc.CreateElement("Content");
            doc.AppendChild(Root);

            foreach (FileDBNode n in filedb.Roots)
                Root.AppendChild(SerializeFileDBNode(n));

            stopWatch.Stop();
            Console.WriteLine("FILEDB to XML conversion took: " + stopWatch.Elapsed.TotalMilliseconds);

            return doc; 
        }

        private XmlNode SerializeFileDBNode(FileDBNode n)
        {
            if (n.NodeType == FileDBNodeType.Attrib)
                return AttribToXmlNode((Attrib)n);
            else if (n.NodeType == FileDBNodeType.Tag)
                return TagToXmlNode((Tag)n);
            throw new Exception(); 
        }

        private XmlNode TagToXmlNode(Tag t)
        {
            String Name = t.GetID();
            var node =  doc.CreateElement(Name);

            foreach (FileDBNode n in t.Children)
                node.AppendChild(SerializeFileDBNode(n));
            return node;
        }

        private XmlNode AttribToXmlNode(Attrib a)
        {
            String Name = a.GetID();
            var node = doc.CreateElement(Name);

            //write empty attribs as <name></name> instead of <Name />
            if (a.Bytesize > 0)
            {
                node.InnerText = HexHelper.ByteArrayToString(a.Content);
            }
            else node.InnerText = "";
            return node;
        }

    }
}
