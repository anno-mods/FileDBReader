using FileDBReader;
using FileDBReader.src;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace FileDBReader
{
    /// <summary>
    /// converts all text in an xml file into hex strings using conversion rules set up in an external xml file
    /// </summary>
    public class XmlExporter
    {
        public XmlExporter() {
            
        }

        /// <summary>
        /// Exports an xmldocument and returns the resulting xmldocument
        /// </summary>
        /// <param name="docpath">path of the input document</param>
        /// <param name="interpreterPath">path of the interpreterfile</param>
        /// <returns>the resulting document</returns>
        public XmlDocument Export(String docpath, String interpreterPath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(docpath);
            return Export(doc, new Interpreter(Interpreter.ToInterpreterDoc(interpreterPath)));
        }

        public XmlDocument Export(XmlDocument doc, Interpreter Interpreter)
        {
            foreach (KeyValuePair<String, Conversion> k in Interpreter.Conversions)
            {
                try {
                    var Nodes = doc.SelectNodes(k.Key);
                    ConvertNodeSet(Nodes.Cast<XmlNode>(), k.Value);
                }
                catch (IOException)
                {
                    Console.WriteLine("I don't even know what this is for. This is an error message even dumber than the old one. Modders gonna take over the world!!");
                }
            }

            if (Interpreter.HasDefaultType())
            {
                String Inverse = Interpreter.GetInverseXPath();
                var Base = doc.SelectNodes("//*[text()]");
                var toFilter = doc.SelectNodes(Inverse);
                var defaults = HexHelper.ExceptNodelists(Base, toFilter);
                ConvertNodeSet(defaults, Interpreter.DefaultType);
            }

            foreach (InternalCompression comp in Interpreter.InternalCompressions)
            {
                var Nodes = doc.SelectNodes(comp.Path);
                foreach (XmlNode node in Nodes)
                {
                    Writer fileWriter = new Writer();

                    var contentNode = node.SelectSingleNode("./Content");
                    XmlDocument xmldoc = new XmlDocument();
                    XmlNode f = xmldoc.ImportNode(contentNode, true);
                    xmldoc.AppendChild(xmldoc.ImportNode(f, true));

                    var stream = fileWriter.Write(xmldoc, new MemoryStream(), comp.CompressionVersion);

                    //Convert This String To Hex Data
                    node.InnerText = HexHelper.StreamToHexString(stream);

                    //try to overwrite the bytesize since it's always exported the same way
                    var ByteSize = node.SelectSingleNode("./preceding-sibling::ByteCount");
                    if (ByteSize != null)
                    {
                        long BufferSize = stream.Length;
                        Type type = typeof(int);
                        ByteSize.InnerText = HexHelper.ByteArrayToString(ConverterFunctions.ConversionRulesExport[type](BufferSize.ToString(), new UnicodeEncoding()));
                    }
                }
            }

            return doc; 
        }

        private void ConvertNodeSet(IEnumerable<XmlNode> matches, Conversion Conversion)
        {
            foreach (XmlNode match in matches)
            {
                try
                {
                    switch (Conversion.Structure)
                    {
                        case ContentStructure.List:
                            exportAsList(match, Conversion.Type, Conversion.Encoding, false);
                            break;
                        case ContentStructure.Default:
                            ExportSingleNode(match, Conversion.Type, Conversion.Encoding, Conversion.Enum, false);
                            break;
                        case ContentStructure.Cdata:
                            exportAsList(match, Conversion.Type, Conversion.Encoding, true);
                            break;
                    }
                }
                catch (InvalidConversionException e)
                {
                    Console.WriteLine("Invalid Conversion at: {1}, Data: {0}, Target Type: {2}", e.ContentToConvert, e.NodeName, e.TargetType);
                }
                
            }
        }


        private void exportAsList(XmlNode n, Type type, Encoding e, bool RespectCdata) {
            //don't do anything with empty nodes
            if (!n.InnerText.Equals("")) 
            {
                String text = n.InnerText;
                if (RespectCdata)
                    text = text.Substring(6, text.Length - 7);
                String[] arr = text.Split(" ");
                if (!arr[0].Equals(""))
                {
                    //use stringbuilder and for loop for performance reasons
                    StringBuilder sb = new StringBuilder("");
                    for (int i = 0; i < arr.Length; i++)
                    {
                        String s = arr[i];
                        try
                        {
                            sb.Append(HexHelper.ByteArrayToString(ConverterFunctions.ConversionRulesExport[type](s, e)));
                        }
                        catch (Exception)
                        {
                            throw new InvalidConversionException(type, n.Name, "List Value");
                        }
                    }
                    String result = sb.ToString();
                    if (RespectCdata)
                        result = "CDATA[" + result + "]";
                    n.InnerText = result;
                }
            }

            
            
        }

        private void ExportSingleNode(XmlNode n, Type type, Encoding e, RuntimeEnum Enum, bool RespectCdata) {
            String Text;

            if (!Enum.IsEmpty())
            {
                Text = Enum.GetKey(n.InnerText);
            }
            else 
            {
                Text = n.InnerText;
            }

            if (RespectCdata)
                Text = Text.Substring(6, Text.Length - 7);

            byte[] converted;
            try
            {
                converted = ConverterFunctions.ConversionRulesExport[type](Text, e);
            }
            catch (Exception)
            {
                throw new InvalidConversionException(type, n.Name, n.InnerText);
            }
            String hex = HexHelper.ByteArrayToString(converted);

            if (RespectCdata) 
                hex = "CDATA[" + HexHelper.ByteArrayToString(BitConverter.GetBytes(hex.Length/2))+ hex + "]";

            n.InnerText = hex;
        }
    }
}
