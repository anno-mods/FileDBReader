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

        #region ToFileDB
        public T ToFileDb<T>(XmlDocument xml) where T: FileDBDocument, new()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            T filedb = new T();

            //skip content node
            foreach (XmlNode n in xml.FirstChild.ChildNodes)
            {
                filedb.Roots.Add(DeserializeFileDBNode(ref filedb, n, null));
            }

            stopWatch.Stop();
            Console.WriteLine("XML to FILEDB conversion took: " + stopWatch.Elapsed.TotalMilliseconds);
            return filedb; 
        }

        private FileDBNode DeserializeFileDBNode<T>(ref T filedb, XmlNode n, Tag parent) where T : FileDBDocument
        {
            if (n.NodeType == XmlNodeType.Text)
                return DeserializeAttrib(ref filedb, n, parent);
            else
                return DeserializeTag(ref filedb, n, parent);
        }

        private Attrib DeserializeAttrib<T>(ref T filedb, XmlNode n, Tag parent) where T : FileDBDocument
        {
            String Text = n.InnerText;
            byte[] Content = new byte[Text.Length / 2];
            using (XmlReader reader = new XmlNodeReader(n))
            {
                reader.ReadContentAsBinHex(Content, 0, Content.Length); 
            }
            return new Attrib() { ID = 0, Content = Content, Parent = parent };
        }

        private Tag DeserializeTag<T>(ref T filedb, XmlNode n, Tag parent) where T : FileDBDocument
        {
            var tag = new Tag() { ID = 0 };
            foreach (XmlNode child in n.ChildNodes)
            {
                tag.Children.Add(DeserializeFileDBNode<T>(ref filedb, child, tag));
            }
            return tag; 
        }

        #endregion

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
            using (XmlWriter writer = node.CreateNavigator().AppendChild())
            {
                writer.WriteBinHex(a.Content, 0, a.Bytesize);
            }
            return node;
        }

    }
}
