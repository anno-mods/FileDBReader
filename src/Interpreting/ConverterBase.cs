using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileDBReader.src
{
    public abstract class ConverterBase
    {
        protected Interpreter Interpreter;
        protected XmlDocument DocumentToConvert;
        protected XmlDocumentMarking Marking;

        public ConverterBase(XmlDocument documentToConvert, Interpreter interpreter)
        {
            Interpreter = interpreter;
            DocumentToConvert = documentToConvert;         
        }

        public abstract XmlDocument Run();

        public void DefaultType()
        {
            var nodes = Marking.GetUnmarkedTextNodes();
            ConvertNodeSet(nodes, Interpreter.DefaultType);
        }

        public void InterpretConversion(String path, Conversion conv)
        {
            var Nodes = DocumentToConvert.SelectNodes(path)?.Cast<XmlNode>().ToArray();
            ConvertNodeSet(Nodes, conv);
        }

        public void ConvertNodeSet(IEnumerable<XmlNode> matches, Conversion conversion)
        {
            foreach (XmlNode match in matches)
            {
                ConvertSingleNode(match, conversion);
            }
        }

        protected void ConvertSingleNode(XmlNode match, Conversion conversion)
        {
            if (match.InnerText.Equals(""))
            {
                Console.WriteLine($"Empty node selected: {match.Name}");
                return;
            }

            if (Marking.IsMarked(match))
            {
                Console.WriteLine($"Node selected by multiple paths: {match.Name}");
                return;
            }

            Marking.Mark(match);

            try
            {
                switch (conversion.Structure)
                {
                    case ContentStructure.List:
                        AsList(match, conversion.Type, conversion.Encoding, false);
                        break;
                    case ContentStructure.Default:
                        SingleNode(match, conversion.Type, conversion.Encoding, conversion.Enum, false);
                        break;
                    case ContentStructure.Cdata:
                        AsList(match, conversion.Type, conversion.Encoding, true);
                        break;
                }
            }
            catch (InvalidConversionException e)
            {
                Console.WriteLine("Invalid Conversion at: {1}, Data: {0}, Target Type: {2}", e.ContentToConvert, e.NodeName, e.TargetType);
            }
        }

        abstract protected void InternalFileDB(InternalCompression comp);

        abstract protected void AsList(XmlNode n, Type type, Encoding e, bool FilterCDATA);

        abstract protected void SingleNode(XmlNode n, Type type, Encoding enc, RuntimeEnum enu, bool FilterCDATA);
    }
}
