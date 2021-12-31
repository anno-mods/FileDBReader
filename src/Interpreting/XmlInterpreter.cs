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
                InterpretInternalFileDB(comp, ref doc);
            }

            //Dictionary stores Path -> Conversion
            foreach ( (String path, Conversion conv) in Interpreter.Conversions)
            {
                InterpretConversion(path, conv, ref doc);
            }

            if (Interpreter.HasDefaultType())
            {
                InterpretDefaultType(Interpreter, ref doc);
            }
            return doc;
        }

        private void InterpretDefaultType(Interpreter Interpreter, ref XmlDocument doc)
        {
            String Inverse = Interpreter.GetCombinedXPath();
            var Base = doc.SelectNodes("//*[text()]");
            var toFilter = doc.SelectNodes(Inverse);
            var defaults = Base.FilterOut(toFilter);
            ConvertNodeSet(defaults, Interpreter.DefaultType);
        }

        private void InterpretConversion(String path, Conversion conv, ref XmlDocument doc)
        {
            var Nodes = doc.SelectNodes(path);
            ConvertNodeSet(Nodes.Cast<XmlNode>(), conv);
        }

        private void InterpretInternalFileDB(InternalCompression comp, ref XmlDocument doc)
        {
            //Register All the nodes by merging dictionaries
            InvalidTagNameHelper.RegisterReplaceOperations(comp.ReplacementOps);

            var nodes = doc.SelectNodes(comp.Path);
            foreach (XmlNode node in nodes)
            {
                var bytearr = HexHelper.BytesFromBinHex(node.InnerText);
                var filereader = new Reader();
                using (MemoryStream ms = new MemoryStream(bytearr))
                {
                    var decompressed = filereader.Read(ms);
                    node.InnerText = "";
                    node.AppendChild(doc.ImportNode(decompressed.DocumentElement, true));
                }
            }

            InvalidTagNameHelper.UnregisterReplaceOperations(comp.ReplacementOps);
        }

        private void ConvertNodeSet(IEnumerable<XmlNode> matches, Conversion Conversion)
        {
            foreach (XmlNode match in matches)
            {
                try
                {
                    if (!match.InnerText.Equals(""))
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
                //make a bytesize check
                if( BytesizeCheck(type, BinaryData.Length, n.Name))
                {
                    //filter out CDATA from the string if needed!
                    if (!BinaryData.Equals("") && FilterCDATA) BinaryData = BinaryData.Substring(6, BinaryData.Length - 7);

                    String s = ConverterFunctions.ConversionRulesImport[type](BinaryData, e);
                    if (!Enum.IsEmpty())
                    {
                        s = Enum.GetValue(s);
                    }
                    //re-add cdata to the string if needed!
                    if (!BinaryData.Equals("") && FilterCDATA) s = "CDATA[" + s + "]";
                    n.InnerText = s;
                }              
            }
            catch (Exception)
            { 
                throw new InvalidConversionException(type, n.Name, "List Value");
            }
        }
        private bool BytesizeCheck(Type t, int BinHexLength, String NodeNameForErrorMessage)
        {
            if (t != typeof(String))
            {
                var ExpectedBytesize = Marshal.SizeOf(t);
                if (t == typeof(Boolean))
                {
                    if (BinHexLength != 2)
                    {
                        Console.WriteLine("Wrong Boolean Bytesize at: {0}. The node is ignored for that reason", NodeNameForErrorMessage);
                        return false;
                    }
                }
                else if (ExpectedBytesize != (BinHexLength / 2))
                {
                    Console.WriteLine("Wrong Bytesize at {0}, Bytesize according to Interpreter: {1}, Found in File: {2}. The node is ignored for that reason.", NodeNameForErrorMessage, ExpectedBytesize, BinHexLength / 2);
                    return false;
                }
            }
            return true;
        }

    }
}
