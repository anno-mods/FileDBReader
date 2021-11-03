using FileDBReader.src;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
    public class XmlInterpreter
    {

        public XmlInterpreter() {

        }

        public XmlDocument Interpret(XmlDocument doc, Interpreter Interpreter)
        {
            //Convert Internal FileDBs before conversion
            foreach (InternalCompression comp in Interpreter.InternalCompressions)
            {
                var nodes = doc.SelectNodes(comp.Path);
                foreach (XmlNode node in nodes)
                {
                    var bytearr = HexHelper.StringToByteArray(node.InnerText);
                    var filereader = new Reader();
                    using (MemoryStream ms = new MemoryStream(bytearr))
                    {
                        var decompressed = filereader.Read(ms);
                        node.InnerText = "";
                        node.AppendChild(doc.ImportNode(decompressed.DocumentElement, true));
                    }
                }
            }

            //Dictionary stores Path -> Conversion
            foreach (KeyValuePair<String, Conversion> k in Interpreter.Conversions)
            {
                try
                {
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
                            InterpretAsList(match, Conversion.Type, false);
                            break;
                        case ContentStructure.Default:
                            InterpretSingleNode(match, Conversion.Type, Conversion.Encoding, Conversion.Enum, false);
                            break;
                        case ContentStructure.Cdata:
                            InterpretAsList(match, Conversion.Type, true);
                            break;
                    }
                }
                catch (InvalidConversionException e)
                {
                    Console.WriteLine("Invalid Conversion at: {1}, Data: {0}, Target Type: {2}", e.ContentToConvert, e.NodeName, e.TargetType);
                }
            }
        }

        private void InterpretAsList(XmlNode n, Type type, bool FilterCDATA)
        {
            try
            {
                String BinaryData = n.InnerText;
                //filter out CDATA
                if (BinaryData != "" && FilterCDATA) BinaryData = BinaryData.Substring(6, BinaryData.Length - 7);

                String s = ConverterFunctions.ListFunctionsInterpret[type](BinaryData);
                if (BinaryData != "" && FilterCDATA) s = "CDATA[" + s + "]";
                n.InnerText = s;
            }
            catch (ArgumentOutOfRangeException) 
            {
                Console.WriteLine("broken CDATA section.");
                throw new InvalidConversionException(type, n.Name, "List Value");
            }
            catch (Exception)
            {
                throw new InvalidConversionException(type, n.Name, "List Value");
            }
        }

        private void InterpretSingleNode(XmlNode n, Type type, Encoding e, RuntimeEnum Enum, bool FilterCDATA)
        {
            try
            {
                String BinaryData = n.InnerText;
                //filter out CDATA from the string
                if (BinaryData != "" && FilterCDATA) BinaryData = BinaryData.Substring(6, BinaryData.Length - 7);

                //make a bytesize check
                int ExpectedBytesize = 0;
                int StringSize = BinaryData.Length;
                if (type != typeof(String))
                {
                    ExpectedBytesize = Marshal.SizeOf(type);
                    if (type == typeof(Boolean)) {
                        if (StringSize != 2) { 
                            Console.WriteLine("Wrong Bytesize at {0}", n.Name);
                        }
                    }
                    else if (ExpectedBytesize != (StringSize / 2))
                    {
                        Console.WriteLine("Wrong Bytesize at {0}, Interpreter says: {1}, Found in File: {2}", n.Name, ExpectedBytesize, StringSize/2);
                    }
                }
                
                String s = ConverterFunctions.ConversionRulesImport[type](BinaryData, e);

                if (!Enum.IsEmpty()) {
                    s = Enum.GetValue(s);
                }
                //readd cdata to the string
                if (!BinaryData.Equals("") && FilterCDATA) s = "CDATA[" + s + "]";
                n.InnerText = s;
            }
            catch (Exception)
            { 
                throw new InvalidConversionException(type, n.Name, "List Value");
            }
        }
    }
}
