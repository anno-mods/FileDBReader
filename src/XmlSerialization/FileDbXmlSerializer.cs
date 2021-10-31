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
        
        public FileDBDocument ToFileDb(XmlDocument xml)
        {
            throw new NotImplementedException();
        }

        public XmlDocument ToXml(FileDBDocument filedb)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            XmlDocument doc = new XmlDocument();
            var Root = doc.CreateElement("Content");
            doc.AppendChild(Root);

            foreach (FileDBNode n in filedb.Roots)
                Root.AppendChild(SerializeFileDBNode(ref doc, n));

            //then let an xml doc load this shit
            doc.Save("test.xml");

            stopWatch.Stop();
            Console.WriteLine("FILEDB to XML conversion took: " + stopWatch.Elapsed.TotalMilliseconds);

            return doc; 
        }

        private XmlNode SerializeFileDBNode(ref XmlDocument doc, FileDBNode n)
        {
            if (n is Attrib)
                return AttribToXmlNode(ref doc, (Attrib)n);
            else if (n is Tag)
                return TagToXmlNode(ref doc, (Tag)n);
            throw new Exception(); 
        }

        private XmlNode TagToXmlNode(ref XmlDocument doc, Tag t)
        {
            String Name = t.GetID();
            var node =  doc.CreateElement(Name);

            foreach (FileDBNode n in t.Children)
                node.AppendChild(SerializeFileDBNode(ref doc, n));
            return node;
        }

        private XmlNode AttribToXmlNode(ref XmlDocument doc, Attrib a)
        {
            String Name = a.GetID();
            var node = doc.CreateElement(Name);
            node.InnerText = HexHelper.ByteArrayToString(a.Content);
            return node; 
        }

    }
}
