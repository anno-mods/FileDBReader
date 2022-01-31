using FileDBSerializing;
using System;
using System.Diagnostics;
using System.Xml;

namespace FileDBReader.src.XmlRepresentation
{
    public class XmlFileDbConverter<T> where T : IFileDBDocument, new()
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
                    filedb.Roots.Add(XmlNodeToFileDBNode(n, null));
                }
            }
            else throw new InvalidXmlDocumentInputException("The XML document provided is missing a root Node! Conversion was terminated!");

            stopWatch.Stop();
            Console.WriteLine("XML to FILEDB conversion took: " + stopWatch.Elapsed.TotalMilliseconds);
            return filedb;
        }

        private FileDBNode XmlNodeToFileDBNode( XmlNode n, Tag parent)
        {
            //This is the closest we can determine this shit.
            if ((n.FirstChild != null && n.FirstChild.NodeType == XmlNodeType.Text))
                return XmlNodeToAttrib(n, parent);
            else
                return XmlNodeToTag(n, parent);
        }

        private Attrib XmlNodeToAttrib(XmlNode n, Tag parent)
        {
            var Content = HexHelper.BytesFromBinHex(n.InnerText);
            var Attrib = filedb.AddAttrib(InvalidTagNameHelper.GetReverseCorrection(n.Name));
            Attrib.Content = Content;
            Attrib.Parent = parent;
            return Attrib;
        }

        private Tag XmlNodeToTag(XmlNode n, Tag parent)
        {
            var tag = filedb.AddTag(InvalidTagNameHelper.GetReverseCorrection(n.Name));
            tag.Parent = parent;
            
            foreach (XmlNode child in n.ChildNodes)
            {
                tag.Children.Add(XmlNodeToFileDBNode(child, tag));
            }
            return tag;

        }
    }
}
