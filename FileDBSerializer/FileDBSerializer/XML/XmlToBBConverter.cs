using System;
using System.Diagnostics;
using System.Xml;
using AnnoMods.BBDom.IO;
using AnnoMods.BBDom.Util;

namespace AnnoMods.BBDom.XML
{
    internal class XmlToBBConverter
    {
        BBDocument doc;

        public XmlToBBConverter()
        {
        }

        public BBDocument ToBBDocument(XmlDocument xml)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            doc = new BBDocument(); 

            //skip content node
            if (xml.HasChildNodes)
            {
                foreach (XmlNode n in xml.FirstChild.ChildNodes)
                {
                    doc.Roots.Add(XmlNodeToBBNode(n, null));
                }
            }
            else throw new InvalidXmlDocumentInputException("The XML document provided is missing a root Node! Conversion was terminated!");

            stopWatch.Stop();
            Console.WriteLine("XML to BB conversion took: " + stopWatch.Elapsed.TotalMilliseconds + " ms");
            return doc;
        }

        private BBNode XmlNodeToBBNode(XmlNode n, Tag parent)
        {
            //Attrib
            if ((n.FirstChild != null && n.FirstChild.NodeType == XmlNodeType.Text) || 
                (n is XmlElement elem && elem.FirstChild == null && elem.IsEmpty == false))
                return XmlNodeToAttrib(n, parent);
            //Tag
            return XmlNodeToTag(n, parent);
        }

        private Attrib XmlNodeToAttrib(XmlNode n, Tag parent)
        {
            var Content = HexHelper.ToBytes(n.InnerText);
            var Attrib = doc.CreateAttrib(InvalidTagNameHelper.GetReverseCorrection(n.Name));
            Attrib.Content = Content;
            Attrib.Parent = parent;
            return Attrib;
        }

        private Tag XmlNodeToTag(XmlNode n, Tag parent)
        {
            var tag = doc.CreateTag(InvalidTagNameHelper.GetReverseCorrection(n.Name));
            tag.Parent = parent;
            
            foreach (XmlNode child in n.ChildNodes)
            {
                tag.Children.Add(XmlNodeToBBNode(child, tag));
            }
            return tag;
        }
    }
}
