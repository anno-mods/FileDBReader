using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using FileDBSerializing;

namespace FileDBReader.src.XmlRepresentation
{
    public class FileDbXmlConverter
    {
        XmlDocument doc; 
        public XmlDocument ToXml(IFileDBDocument filedb)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            doc = new XmlDocument();

            var Root = doc.CreateElement("Content");
            doc.AppendChild(Root);

            ConvertCollection(Root, filedb.Roots);                

            stopWatch.Stop();
            Console.WriteLine("FILEDB to XML conversion took: " + stopWatch.Elapsed.TotalMilliseconds);

            return doc; 
        }

        /// <summary>
        /// Serializes the single Node <paramref name="n"/> to an xml node. Returns
        /// </summary>
        /// <param name="n"></param>
        /// <param name="node">the XML node result, if provided. </param>
        /// <returns>True if the serialization was successful</returns>
        /// <exception cref="Exception"><paramref name="n"/> is of an unknown Node Type</exception>
        private bool TryConstructXmlNodeFromFileDBNode(FileDBNode n, out XmlNode node)
        {
            if (n.NodeType == FileDBNodeType.Attrib)
                return (node = AttribToXmlNode((Attrib)n)) is XmlNode;
            else if (n.NodeType == FileDBNodeType.Tag)
                return (node = TagToXmlNode((Tag)n)) is XmlNode;
            throw new InvalidOperationException(); 
        }

        /// <summary>
        /// Serializes a Collection of FileDBNodes and adds them to their <paramref name="parent">parent XML node</paramref>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="children"></param>
        private void ConvertCollection(XmlNode parent, IEnumerable<FileDBNode> children)
        {
            foreach (FileDBNode n in children)
            {
                if (TryConstructXmlNodeFromFileDBNode(n, out var childxmlnode))
                    parent.AppendChild(childxmlnode);
            }
        }

        /// <summary>
        /// Tries to create a new XmlElement in the Serializers document with the given <paramref name="Nodename"/>. 
        /// </summary>
        /// <param name="Nodename"></param>
        /// <param name="node"></param>
        /// <returns>True if the creation was successful.</returns>
        private bool TryCreateNode(String Nodename, out XmlNode node)
        {
            try
            {
                node = doc.CreateElement(InvalidTagNameHelper.GetCorrection(Nodename));
                return true;
            }
            catch (XmlException e)
            {
                Console.WriteLine($"Could not create Node: {Nodename}. The Node and it's children are ignored for this reason. You can replace the Nodename using the -z option.");
                node = null;
                return false;
            }
        }

        private XmlNode? TagToXmlNode(Tag t)
        {
            String Name = t.GetID();
            if (TryCreateNode(Name, out var node))
            {
                ConvertCollection(node, t.Children);
                return node;
            }
            return null;            
        }

        private XmlNode? AttribToXmlNode(Attrib a)
        {
            String Name = a.GetID();
            if (TryCreateNode(Name, out var node))
            {
                node.InnerText = a.Bytesize > 0 ? node.InnerText = HexHelper.ToBinHex(a.Content) : "";
                return node;
            }
            else return null;
        }

    }
}
