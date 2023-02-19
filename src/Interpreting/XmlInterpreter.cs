using FileDBReader.src;
using FileDBReader.src.XmlRepresentation;
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
    /// <summary>
    /// converts hex strings in an xml file into their types using conversion rules set up in an external xml file.
    /// </summary>
    public class XmlInterpreter : ConverterBase
    {
        public XmlInterpreter(XmlDocument document, Interpreter interpreter) : base(document, interpreter) { }

        protected override void InternalFileDB(InternalCompression comp)
        {
            //Register All the nodes by merging dictionaries
            InvalidTagNameHelper.RegisterReplaceOperations(comp.ReplacementOps);

            var nodes = DocumentToConvert.SelectNodes(comp.Path);
            foreach (XmlNode node in nodes)
            {
                var bytearr = HexHelper.ToBytes(node.InnerText);
                var filereader = new Reader();
                using (MemoryStream ms = new MemoryStream(bytearr))
                {
                    var decompressed = filereader.Read(ms);
                    node.InnerText = "";
                    node.AppendChild(DocumentToConvert.ImportNode(decompressed.DocumentElement, true));
                }
            }

            InvalidTagNameHelper.UnregisterReplaceOperations(comp.ReplacementOps);
        }

        protected override void AsList(XmlNode n, Type type, Encoding e, bool FilterCDATA)
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

        protected override void SingleNode(XmlNode n, Type type, Encoding e, RuntimeEnum Enum, bool FilterCDATA)
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
