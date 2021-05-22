using FileDBReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace FileDBReader
{
    /// <summary>
    /// converts all text in an xml file into hex strings using conversion rules set up in an external xml file
    /// </summary>
    class XmlExporter
    {
        public XmlExporter() {
            
        }

        public void Export(String docpath, String interpreterPath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(docpath);

            XmlDocument interpreter = new XmlDocument();
            interpreter.Load(interpreterPath);

            Export(doc, interpreter, docpath);
        }

        public void Export(XmlDocument doc, XmlDocument interpreter, String docpath) {
            //default type
            Type defaultType = null;

            var attrib = interpreter.SelectSingleNode("/Converts/Default").Attributes["Type"];
            if (attrib != null)
            {
                defaultType = Type.GetType("System." + attrib.Value);
            }

            var converts = interpreter.SelectNodes("/Converts/Converts/Convert");

            //Convert internal FileDBs before conversion of the rest
            var internalFileDBs = interpreter.SelectNodes("/Converts/InternalCompression/Element");
            foreach (XmlNode n in internalFileDBs)
            {
                var nodes = doc.SelectNodes(n.Attributes["Path"].Value);

                foreach (XmlNode node in nodes)
                {
                    var contentNode = node.SelectSingleNode("./Content");
                    //convert shit
                    XmlDocument xmldoc = new XmlDocument();
                    XmlNode f = xmldoc.ImportNode(contentNode, true);
                    xmldoc.AppendChild(xmldoc.ImportNode(f, true));

                    FileWriter fileWriter = new FileWriter();
                    var stream = fileWriter.Export(xmldoc, new MemoryStream());

                    node.InnerText = ByteArrayToString(HexHelper.StreamToByteArray(stream));

                    //try to overwrite the bytesize of the thing
                    var ByteSize = node.SelectSingleNode("./preceding-sibling::ByteCount");
                    if (ByteSize != null) {
                        long BufferSize = stream.Length;
                        Type type = typeof(int);
                        ByteSize.InnerText = ByteArrayToString(ConverterFunctions.ConversionRulesExport[type](BufferSize.ToString(), new UnicodeEncoding()));
                    }
                }
            }

            foreach (XmlNode x in converts) {
                var type = Type.GetType("System." + x.Attributes["Type"].Value);
                String Path = x.Attributes["Path"].Value;

                var Nodes = doc.SelectNodes(Path);
                foreach (XmlNode match in Nodes)
                {
                    //make unicode as the default encoding
                    Encoding encoding = new UnicodeEncoding();

                    //if another encoding is specified, take that
                    if (x.Attributes["Encoding"] != null)
                    {
                        encoding = Encoding.GetEncoding(x.Attributes["Encoding"].Value);
                    }

                    //make pass the object down together with the type
                    String Text = match.InnerText;
                    byte[] converted = ConverterFunctions.ConversionRulesExport[type](Text, encoding);
                    String hex = ByteArrayToString(converted);
                    match.InnerText = hex;
                }
            }
            if (defaultType != null) {
                //get a combined path of all
                String xPath = "";
                bool isFirst = true;
                foreach (XmlNode convert in converts)
                {
                    if (!isFirst)
                    {
                        xPath += " | ";
                    }
                    xPath += convert.Attributes["Path"].Value;
                    isFirst = false;
                }
                //select all text not in combined path#
                var Base = doc.SelectNodes("//*[text()]");
                var toFilter = doc.SelectNodes(xPath);
                
                var defaults = HexHelper.ExceptNodelists(Base, toFilter);

                //convert that to default type
                foreach (XmlNode y in defaults)
                {
                    Encoding encoding = new UnicodeEncoding();
                    try
                    {
                        //make pass the object down together with the type
                        String Text = y.InnerText;
                        byte[] converted = ConverterFunctions.ConversionRulesExport[defaultType](Text, encoding);
                        String hex = ByteArrayToString(converted);
                        y.InnerText = hex;
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(docpath, "_e"), "xml"));
        }

        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        
    }
}
