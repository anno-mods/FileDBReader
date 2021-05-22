using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace FileDBReader
{
    //expected structure of interpreter
    /*<Converts>
          <Default Type ="int" />
          <Converts>
            <Convert Path ="//Text" Type ="String" />
          </Converts>
        </Converts>
     */

    /// <summary>
    /// converts hex strings in an xml file into their types using conversion rules set up in an external xml file.
    /// </summary>
    class XmlInterpreter
    {
        
        public XmlInterpreter() {
            
        }

        public void Interpret(String docPath, String InterpreterPath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(docPath);

            XmlDocument interpreter = new XmlDocument();
            interpreter.Load(InterpreterPath);

            Interpret(doc, interpreter, docPath);
        }

        public void Interpret(XmlDocument doc, XmlDocument interpreter, String docpath) {
            //default type
            Type defaultType = null;

            var attrib = interpreter.SelectSingleNode("/Converts/Default").Attributes["Type"];
            if (attrib != null) {
                defaultType = Type.GetType("System." + attrib.Value);
            }

            //Convert internal FileDBs before conversion
            var internalFileDBs = interpreter.SelectNodes("/Converts/InternalCompression/Element");
            foreach (XmlNode n in internalFileDBs) {

                var nodes = doc.SelectNodes(n.Attributes["Path"].Value);

                foreach (XmlNode node in nodes) {
                    var span = HexHelper.toByteSpan(node.InnerText);
                    var filereader = new FileReader();
                    var decompressed = filereader.ReadSpan(span);
                    //add the decompressed document to the current documentConverterFunctions.ConversionRulesImport
                    decompressed.Save("decompressed.xml");

                    node.InnerText = "";
                    node.AppendChild(doc.ReadNode(decompressed.Root.CreateReader()) as XmlElement);
                }
            }

            //converts
            var converts = interpreter.SelectNodes("/Converts/Converts/Convert");

            foreach (XmlNode x in converts) {
                var type = Type.GetType("System." + x.Attributes["Type"].Value);

                String Path = x.Attributes["Path"].Value;

                var Nodes = doc.SelectNodes(Path);
                foreach (XmlNode match in Nodes) {

                    //make unicode as the default encoding
                    Encoding encoding = new UnicodeEncoding();

                    //if another encoding is specified, take that
                    if (x.Attributes["Encoding"] != null)
                    {
                        encoding = Encoding.GetEncoding(x.Attributes["Encoding"].Value);
                    }

                    //look for List arg, if list then split, else use normal stuff
                    if (x.Attributes["Structure"] != null)
                    {
                        switch (x.Attributes["Structure"].Value)
                        {
                            case "List":
                                String BinaryData = match.InnerText;
                                var result = InterpretAsList(BinaryData, type);
                                match.InnerText = result;
                                break;
                        }
                    }
                    else
                    {
                        String BinaryData = match.InnerText;

                        //encoding gets ignored for int/float etc. in the conversion rules. 
                        String converted = ConverterFunctions.ConversionRulesImport[type](BinaryData, encoding);
                        match.InnerText = converted;
                    }
                }
            }

            //DefaultType
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
                foreach (XmlNode internalFileDB in internalFileDBs) {
                    xPath += " | ";
                    xPath += internalFileDB.Attributes["Path"].Value;
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
                        String BinaryData = y.InnerText;
                        String converted = ConverterFunctions.ConversionRulesImport[defaultType](BinaryData, encoding);
                        y.InnerText = converted;
                    }
                    catch (Exception e)
                    {

                    }
                }
            }   
            
            doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(docpath, "_i"), "xml"));
        }

        private String InterpretAsList(String BinaryData, Type type)
        {
            //get size of target datatype.
            int bytesize = Marshal.SizeOf(type);

            String s = "";
            //performance optimizing supports lists of shorts and bytes atm
            if (type == typeof(short))
            {
                var span = HexHelper.toShortSpan(BinaryData).ToArray();
                s = String.Join<short>(" ", span);
            }
            else if (type == typeof(byte))
            {
                var span = HexHelper.toByteSpan(BinaryData).ToArray();
                s = String.Join<byte>(" ", span);
            }
            else if (type == typeof(int))
            {
                var span = HexHelper.toIntSpan(BinaryData).ToArray();
                s = String.Join<int>(" ", span);
            }
            else if (type == typeof(float))
            {
                var span = HexHelper.toFloatSpan(BinaryData).ToArray();
                s = String.Join<float>(" ", span);
            }
            else if (type == typeof(UInt16))
            {
                var span = HexHelper.toUInt16Span(BinaryData).ToArray();
                s = String.Join<UInt16>(" ", span);
            }
            else { 
                //do it the old fashioned way if there is no performance optimizing
            }

            //assume maps are short spans for the moment :)

            Console.WriteLine("Map complete");

            return s;
        }
    }
}
