using AnnoMods.BBDom.Util;
using AnnoMods.BBDom.XML;
using FileDBReader;
using FileDBReader.src;
using FileDBReader.src.XmlRepresentation;
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
    public class XmlExporter : ConverterBase
    {
        public XmlExporter(XmlDocument document, Interpreter interpreter) : base(document, interpreter) { }

        public override XmlDocument Run()
        {
            Marking = XmlDocumentMarking.InitFrom(DocumentToConvert);
            var paths = Interpreter.InternalCompressions.Select(x => x.Path).ToArray();
            foreach (string path in paths)
            {
                var nodes = DocumentToConvert.SelectNodes(path);
                Marking.Mark(nodes?.Cast<XmlNode>().ToArray() ?? Enumerable.Empty<XmlNode>());
            }

            foreach ((String path, Conversion conv) in Interpreter.Conversions)
            {
                InterpretConversion(path, conv);
            }

            if (Interpreter.HasDefaultType())
                DefaultType();

            foreach (InternalCompression comp in Interpreter.InternalCompressions)
            {
                InternalFileDB(comp);
            }

            return DocumentToConvert;
        }

        protected override void InternalFileDB(InternalCompression comp)
        {
            InvalidTagNameHelper.RegisterReplaceOperations(comp.ReplacementOps);

            var Nodes = DocumentToConvert.SelectNodes(comp.Path);
            foreach (XmlNode node in Nodes)
            {
                Writer fileWriter = new Writer();

                var contentNode = node.SelectSingleNode("./Content");
                XmlDocument xmldoc = new XmlDocument();
                XmlNode f = xmldoc.ImportNode(contentNode, true);
                xmldoc.AppendChild(xmldoc.ImportNode(f, true));

                var stream = fileWriter.Write(xmldoc, new MemoryStream(), comp.CompressionVersion);

                //Convert This String To Hex Data
                node.InnerText = HexHelper.ToBinHex(stream);

                //try to overwrite the bytesize since it's always exported the same way
                var ByteSize = node.SelectSingleNode("./preceding-sibling::ByteCount");
                if (ByteSize != null)
                {
                    long BufferSize = stream.Length;
                    Type type = typeof(int);
                    ByteSize.InnerText = HexHelper.ToBinHex(ConverterFunctions.ConversionRulesExport[type](BufferSize.ToString(), new UnicodeEncoding()));
                }
            }

            InvalidTagNameHelper.UnregisterReplaceOperations(comp.ReplacementOps);
        }

        protected override void AsList(XmlNode n, Type type, Encoding e, bool RespectCdata) {
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
                            sb.Append(HexHelper.ToBinHex(ConverterFunctions.ConversionRulesExport[type](s, e)));
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

        protected override void SingleNode(XmlNode n, Type type, Encoding e, RuntimeEnum Enum, bool RespectCdata) {
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
            String hex = HexHelper.ToBinHex(converted);

            if (RespectCdata) 
                hex = "CDATA[" + HexHelper.ToBinHex(BitConverter.GetBytes(hex.Length/2))+ hex + "]";

            n.InnerText = hex;
        }
    }
}
