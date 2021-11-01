using FileDBSerializing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileDBReader.src.XmlSerialization
{
    class XmlFileDbSerializer<T> where T : FileDBDocument, new()
    {
        T filedb;
        private ushort TagID;
        private ushort AttribID;

        private static readonly ushort TAG_START = 1; //0x01 0x00
        private static readonly ushort ATTRIB_START = 32768; //0x01 0x80

        private void ResetIDCounter()
        {
            TagID = (ushort)(TAG_START + 1);
            AttribID = (ushort)(ATTRIB_START + 1);
        }

        public T ToFileDb(XmlDocument xml)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            filedb = new T();
            ResetIDCounter();

            //skip content node
            if (xml.HasChildNodes)
            {
                foreach (XmlNode n in xml.FirstChild.ChildNodes)
                {
                    filedb.Roots.Add(DeserializeFileDBNode(n, null));
                }
            }
            else throw new InvalidXmlDocumentInputException("The XML document provided is missing a root Node! Conversion was terminated!");

            stopWatch.Stop();
            Console.WriteLine("XML to FILEDB conversion took: " + stopWatch.Elapsed.TotalMilliseconds);
            return filedb;
        }

        private FileDBNode DeserializeFileDBNode( XmlNode n, Tag parent)
        {
            if (n.NodeType == XmlNodeType.Text)
                return DeserializeAttrib(n, parent);
            else
                return DeserializeTag(n, parent);
        }

        private Attrib DeserializeAttrib(XmlNode n, Tag parent)
        {
            String Text = n.InnerText;
            byte[] Content = new byte[Text.Length / 2];
            using (XmlReader reader = new XmlNodeReader(n))
            {
                reader.ReadContentAsBinHex(Content, 0, Content.Length);
            }

            ushort ID = ATTRIB_START;
            if (!n.Name.Equals("None") && !filedb.Tags.Attribs.ContainsValue(n.Name))
            {
                filedb.Tags.Attribs.Add(AttribID++, n.Name);
            }
            return new Attrib() { ID = ID, Content = Content, Parent = parent, ParentDoc = filedb };
        }

        private Tag DeserializeTag(XmlNode n, Tag parent)
        {
            ushort ID = TAG_START;
            if (!n.Name.Equals("None") && !filedb.Tags.Attribs.ContainsValue(n.Name))
            {
                filedb.Tags.Tags.Add(TagID++, n.Name);
            }

            var tag = new Tag() { ID = 0, Parent = parent, ParentDoc = filedb};
            foreach (XmlNode child in n.ChildNodes)
            {
                tag.Children.Add(DeserializeFileDBNode(child, tag));
            }
            return tag;

        }
    }
}
