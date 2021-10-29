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
                    var span = HexHelper.toSpan<byte>(node.InnerText);
                    var filereader = new FileReader();
                    var decompressed = filereader.ReadSpan(span);
                    node.InnerText = "";
                    node.AppendChild(doc.ImportNode(decompressed.DocumentElement, true));
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
                catch (IOException ex)
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
                switch (Conversion.Structure)
                {
                    case ContentStructure.List:
                        try
                        {
                            InterpretAsList(match, Conversion.Type, false);
                        }
                        catch (InvalidConversionException e)
                        {
                            Console.WriteLine("Invalid Conversion at: {1}, Data: {0}, Target Type: {2}", e.ContentToConvert, e.NodeName, e.TargetType);
                        }
                        break;
                    case ContentStructure.Default:
                        try
                        {
                            InterpretSingleNode(match, Conversion.Type, Conversion.Encoding, Conversion.Enum, false);
                        }
                        catch (InvalidConversionException e)
                        {
                            Console.WriteLine("Invalid Conversion at: {1}, Data: {0}, Target Type: {2}", e.ContentToConvert, e.NodeName, e.TargetType);
                        }
                        break;
                    case ContentStructure.Cdata:
                        try
                        {
                            InterpretAsList(match, Conversion.Type, true);
                        }
                        catch (InvalidConversionException e)
                        {
                            Console.WriteLine("Invalid Conversion at: {1}, Data: {0}, Target Type: {2}", e.ContentToConvert, e.NodeName, e.TargetType);
                        }
                        break; 
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
            catch (ArgumentOutOfRangeException ex) 
            {
                Console.WriteLine("broken CDATA section.");
                throw new InvalidConversionException(type, n.Name, "List Value");
            }
            catch (Exception ex)
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
                if (BinaryData != "" && FilterCDATA) s = "CDATA[" + s + "]";
                n.InnerText = s;
            }
            catch (Exception ex)
            { 
                throw new InvalidConversionException(type, n.Name, "List Value");
            }
        }

        #region DEPRACATED 

        [Obsolete("ConvertNodeSet(IEnumerable<XmlNode> matches, XmlNode ConverterInfo) is deprecated, please use Export(IEnumerable<XmlNode> matches, Conversion c) instead.")]
        private void ConvertNodeSet(IEnumerable<XmlNode> matches, XmlNode ConverterInfo)
        {
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
            //get Cdata
            bool IsCdataNode = false;
            if (ConverterInfo.Attributes["IsCdataNode"] != null)
                IsCdataNode = true;

            //get if it should use Enum

            RuntimeEnum Enum = new RuntimeEnum();
            var EnumEntries = ConverterInfo.SelectNodes("./Enum/Entry");

            if (EnumEntries != null)
            {
                foreach (XmlNode EnumEntry in EnumEntries)
                {
                    try
                    {
                        var Value = EnumEntry.Attributes["Value"];
                        var Name = EnumEntry.Attributes["Name"];
                        if (Value != null && Name != null)
                        {
                            Enum.AddValue(Value.Value, Name.Value);
                        }
                        else
                        {
                            Console.WriteLine("An XML Node Enum Entry was not defined correctly. Please check your interpreter file if every EnumEntry has an ID and a Name");
                        }
                    }
                    catch (NullReferenceException ex)
                    {
                    }
                }
            }

            foreach (XmlNode match in matches)
            {
                switch (Structure)
                {
                    case "List":
                        try
                        {
                            InterpretAsList(match, type, IsCdataNode);
                        }
                        catch (InvalidConversionException e)
                        {
                            Console.WriteLine("Invalid Conversion at: {1}, Data: {0}, Target Type: {2}", e.ContentToConvert, e.NodeName, e.TargetType);
                        }
                        break;
                    case "Default":
                        try
                        {
                            InterpretSingleNode(match, type, encoding, Enum, IsCdataNode);
                        }
                        catch (InvalidConversionException e)
                        {
                            Console.WriteLine("Invalid Conversion at: {1}, Data: {0}, Target Type: {2}", e.ContentToConvert, e.NodeName, e.TargetType);
                        }
                        break;
                }
            }
        }

        [Obsolete("Export(XmlDocument doc, XmlDocument interpreter) is deprecated, please use Interpret(XmlDocument doc, Interpreter i) instead.")]
        public XmlDocument Interpret(XmlDocument doc, XmlDocument interpreter)
        {
            //default type
            XmlNode defaultAttrib = null;
            defaultAttrib = interpreter.SelectSingleNode("/Converts/Default");
            var internalFileDBs = interpreter.SelectNodes("/Converts/InternalCompression/Element");
            var converts = interpreter.SelectNodes("/Converts/Converts/Convert");

            //Convert internal FileDBs before conversion
            foreach (XmlNode n in internalFileDBs)
            {
                var nodes = doc.SelectNodes(n.Attributes["Path"].Value);
                foreach (XmlNode node in nodes)
                {
                    var span = HexHelper.toSpan<byte>(node.InnerText);

                    var filereader = new FileReader();
                    var decompressed = filereader.ReadSpan(span);
                    node.InnerText = "";
                    node.AppendChild(doc.ImportNode(decompressed.DocumentElement, true));
                }
            }

            //converts
            foreach (XmlNode x in converts)
            {
                try
                {
                    String Path = x.Attributes["Path"].Value;
                    var Nodes = doc.SelectNodes(Path);
                    ConvertNodeSet(Nodes, x);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Path not correctly set lol");
                }
            }

            //DefaultType
            if (defaultAttrib != null)
            {
                //get a combined xpath of all
                List<String> StringList = new List<string>();
                foreach (XmlNode convert in converts)
                    StringList.Add(convert.Attributes["Path"].Value);
                foreach (XmlNode internalFileDB in internalFileDBs)
                    StringList.Add(internalFileDB.Attributes["Path"].Value);
                String xPath = String.Join(" | ", StringList);

                //select all text that is not in the combined path
                var Base = doc.SelectNodes("//*[text()]");
                var toFilter = doc.SelectNodes(xPath);
                var defaults = HexHelper.ExceptNodelists(Base, toFilter);
                ConvertNodeSet(defaults, defaultAttrib);
            }

            return doc;
        }


        //f* performance I won't write everything twice :) the cast should not take to long
        [Obsolete]
        private void ConvertNodeSet(XmlNodeList matches, XmlNode ConverterInfo)
        {
            IEnumerable<XmlNode> cast = matches.Cast<XmlNode>();
            ConvertNodeSet(cast, ConverterInfo);
        }

        #endregion
    }
}
