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
    class XmlExporter
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

            XmlDocument interpreter = new XmlDocument();
            interpreter.Load(interpreterPath);

            return Export(doc, interpreter);
        }

        public XmlDocument Export(XmlDocument doc, XmlDocument interpreter) {
            //default type
            XmlNode defaultAttrib = null;
            defaultAttrib = interpreter.SelectSingleNode("/Converts/Default");
            var converts = interpreter.SelectNodes("/Converts/Converts/Convert");
            var internalFileDBs = interpreter.SelectNodes("/Converts/InternalCompression/Element");

            //converts
            foreach (XmlNode x in converts) {
                try
                {
                    String Path = x.Attributes["Path"].Value;
                    var Nodes = doc.SelectNodes(Path);
                    ConvertNodeSet(Nodes, x);
                }
                catch (InvalidConversionException e)
                {
                    Console.WriteLine("Path causes conversion errors: {0} \n NodeName: {1} \n Text to Convert: {2}, Target Type: {3}", x.Attributes["Path"].Value, e.NodeName, e.ContentToConvert, e.TargetType);
                }
            }

            //defaultType
            if (defaultAttrib != null) {
                //get a combined xpath of all
                List<String> StringList = new List<string>();
                foreach (XmlNode convert in converts)
                    StringList.Add(convert.Attributes["Path"].Value);
                foreach (XmlNode internalFileDB in internalFileDBs)
                    StringList.Add(internalFileDB.Attributes["Path"].Value);
                String xPath = String.Join(" | ", StringList);

                //select all text not in combined path#
                var Base = doc.SelectNodes("//*[text()]");
                var toFilter = doc.SelectNodes(xPath);
                var defaults = HexHelper.ExceptNodelists(Base, toFilter);

                //convert that to default type
                ConvertNodeSet(defaults, defaultAttrib);
            }
            //internal filedbs
            foreach (XmlNode n in internalFileDBs)
            {
                var nodes = doc.SelectNodes(n.Attributes["Path"].Value);
                foreach (XmlNode node in nodes)
                {
                    //get internal document
                    var contentNode = node.SelectSingleNode("./Content");
                    XmlDocument xmldoc = new XmlDocument();
                    XmlNode f = xmldoc.ImportNode(contentNode, true);
                    xmldoc.AppendChild(xmldoc.ImportNode(f, true));

                    //compress the document
                    FileWriter fileWriter = new FileWriter();

                    int FileVersion = 1;
                    //get File Version of internal compression
                    var VersionNode = n.Attributes["CompressionVersion"];
                    if (!(VersionNode == null))
                    {
                        FileVersion = Int32.Parse(VersionNode.Value);
                    }
                    else
                    {
                        Console.WriteLine("Your interpreter should specify a version for internal FileDBs. For this conversion, 1 was auto-chosen. Make sure your versions match up!");
                    }

                    var stream = fileWriter.Export(xmldoc, new MemoryStream(), FileVersion);

                    //get this stream to hex 
                    node.InnerText = ByteArrayToString(HexHelper.StreamToByteArray(stream));

                    //try to overwrite the bytesize since it's always exported the same way
                    var ByteSize = node.SelectSingleNode("./preceding-sibling::ByteCount");
                    if (ByteSize != null)
                    {
                        long BufferSize = stream.Length;
                        Type type = typeof(int);
                        ByteSize.InnerText = ByteArrayToString(ConverterFunctions.ConversionRulesExport[type](BufferSize.ToString(), new UnicodeEncoding()));
                    }
                }
            }

            return doc;
            //doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(docpath, "_e"), "xml"));
        }

        private void ConvertNodeSet(XmlNodeList matches, XmlNode ConverterInfo)
        {
            IEnumerable<XmlNode> cast = matches.Cast<XmlNode>();
            ConvertNodeSet(cast, ConverterInfo);
        }

        private void ConvertNodeSet(IEnumerable<XmlNode> matches, XmlNode ConverterInfo) {
            //get type the nodeset should be converted to
            var type = Type.GetType("System." + ConverterInfo.Attributes["Type"].Value);
            //get encoding
            Encoding encoding = new UnicodeEncoding();
            if (ConverterInfo.Attributes["Encoding"] != null)
                encoding = Encoding.GetEncoding(ConverterInfo.Attributes["Encoding"].Value);
            //get structure
            String Structure = "Default";
            if (ConverterInfo.Attributes["Structure"] != null)
                Structure = ConverterInfo.Attributes["Structure"].Value;

            //getEnum
            var EnumEntries = ConverterInfo.SelectNodes("./Enum/Entry");
            RuntimeEnum Enum = new RuntimeEnum();
            if (EnumEntries != null) {
                foreach (XmlNode EnumEntry in EnumEntries)
                {
                    try
                    {
                        var Value = EnumEntry.Attributes["Value"];
                        var Name = EnumEntry.Attributes["Name"];
                        if (Value != null && Name != null)
                        {
                            Enum.AddValue(Name.Value, Value.Value);
                        }
                        else {
                            Console.WriteLine("An XML Node Enum Entry was not defined correctly. Please check your interpreter file if every EnumEntry has an ID and a Name");
                        }
                    }
                    catch (NullReferenceException ex)
                    {
                    }
                }
            }

            foreach (XmlNode node in matches) {
                switch (Structure)
                {
                    case "List":
                        exportAsList(node, type, encoding);
                        break;
                    case "Default":
                        try
                        {
                            ExportSingleNode(node, type, encoding, Enum);
                        }
                        catch (InvalidConversionException e)
                        {
                            Console.WriteLine("Invalid Conversion at: {1}, Data: {0}, Target Type: {2}", e.ContentToConvert, e.NodeName, e.TargetType);
                        }
                        break;
                }
            }
        }

        private void exportAsList(XmlNode n, Type type, Encoding e) {
            //don't do anything with empty nodes
            if (!n.InnerText.Equals("")) 
            {
                String[] arr = n.InnerText.Split(" ");
                if (!arr[0].Equals(""))
                {
                    //use stringbuilder and for loop for performance reasons
                    StringBuilder sb = new StringBuilder("");
                    for (int i = 0; i < arr.Length; i++)
                    {
                        String s = arr[i];
                        try
                        {
                            sb.Append(ByteArrayToString(ConverterFunctions.ConversionRulesExport[type](s, e)));
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidConversionException(type, n.Name, "List Value");
                        }
                    }
                    n.InnerText = sb.ToString();
                }
            }

            
            
        }

        private void ExportSingleNode(XmlNode n, Type type, Encoding e, RuntimeEnum Enum) {
            String Text;

            if (!Enum.IsEmpty())
            {
                Text = Enum.GetValue(n.InnerText);
            }
            else 
            {
                Text = n.InnerText;
            }

            byte[] converted;
            try
            {
                converted = ConverterFunctions.ConversionRulesExport[type](Text, e);
            }
            catch (Exception ex)
            {
                throw new InvalidConversionException(type, n.Name, n.InnerText);
            }
            String hex = ByteArrayToString(converted);
            n.InnerText = hex;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }
    }
}
