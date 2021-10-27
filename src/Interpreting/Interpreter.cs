using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace FileDBReader.src
{
    public enum ContentStructure {Default, List, Cdata};

    /// <summary>
    /// Represents an InterpreterFile
    /// </summary>
    public class Interpreter
    {
        //Attribs
        public List<InternalCompression> InternalCompressions = new List<InternalCompression>();
        public Dictionary<string, Conversion> Conversions = new Dictionary<string, Conversion>();
        public Conversion DefaultType;

        public static XmlDocument ToInterpreterDoc(String InterpreterPath) 
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(InterpreterPath);
            }
            catch (XmlException e)
            {
                Console.WriteLine("Could not load Interpreter at: {0}", InterpreterPath);
            }
            return doc; 
        }

        public String GetInverseXPath()
        {
            List<String> StringList = new List<string>();
            foreach (KeyValuePair<String, Conversion> k in Conversions)
                StringList.Add(k.Key);
            foreach (InternalCompression internalFileDB in InternalCompressions)
                StringList.Add(internalFileDB.Path);
            return String.Join(" | ", StringList);
        }

        public Interpreter(XmlDocument InterpreterDoc)
        {
            XmlNode DefaultAttribNode = null;
            DefaultAttribNode = InterpreterDoc.SelectSingleNode("/Converts/Default");
            var ConvertNodes = InterpreterDoc.SelectNodes("/Converts/Converts/Convert");
            var InternalCompressionNodes = InterpreterDoc.SelectNodes("/Converts/InternalCompression/Element");

            //Conversions
            foreach (XmlNode Convert in ConvertNodes)
            {
                //make a new dictionary entrance
                Conversion c = new Conversion(Convert);
                String Path = GetPath(Convert);

                Conversions.Add(Path, c);
            }

            //Internal Compression Parsing
            foreach (XmlNode InternalCompression in InternalCompressionNodes)
            {
                InternalCompression Compression = new InternalCompression(InternalCompression);
                InternalCompressions.Add(Compression);
            }

            //default type
            if (DefaultAttribNode != null)
            {
                DefaultType = new Conversion(DefaultAttribNode); 
            }
        }

        public bool HasDefaultType()
        {
            return DefaultType != null; 
        }

        private String GetPath(XmlNode ConvertNode)
        {
            return ConvertNode.Attributes["Path"].Value;
        }

    }

    public class Conversion
    {
        public Type Type;
        public ContentStructure Structure;
        public RuntimeEnum Enum = new RuntimeEnum();
        public Encoding Encoding = new UnicodeEncoding();

        public Conversion(XmlNode ConvertNode)
        {
            //gather info from the Conversion Attributes
            var TypeAttr = ConvertNode.Attributes["Type"];
            var StructureAttr = ConvertNode.Attributes["Structure"];
            var EncodingAttr = ConvertNode.Attributes["Encoding"];

            if(TypeAttr != null)
                Type = ToType(TypeAttr.Value);
            if(StructureAttr != null)
                Structure = ToContentStructure(StructureAttr.Value);
            if(EncodingAttr != null)
                Encoding = ToEncoding(EncodingAttr.Value);

            //Enum Parsing
            var EnumNode = ConvertNode.SelectSingleNode("./Enum");
            if (EnumNode != null)
            {
                this.Enum = ToEnum(EnumNode);
            }
        }

        private Type ToType(string typeStr)
        {
            return Type.GetType("System." + typeStr);
        }

        private ContentStructure ToContentStructure(string structureStr)
        {
            if (System.Enum.IsDefined<ContentStructure>(System.Enum.Parse<ContentStructure>(structureStr)))
                return System.Enum.Parse<ContentStructure>(structureStr);
            else
                return ContentStructure.Default;
        }

        private Encoding ToEncoding(string encodingStr)
        {
            return Encoding.GetEncoding(encodingStr);
        }

        private RuntimeEnum ToEnum(XmlNode EnumNode)
        {
            //getEnum
            var EnumEntries = EnumNode.SelectNodes("./Entry");
            RuntimeEnum Enum = new RuntimeEnum();
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
                            Enum.AddValue(Name.Value, Value.Value);
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
            return Enum;
        }

        public bool hasType()
        {
            return Type != null;         
        }

        public bool hasEnum()
        {
            return Enum != null; 
        }

        public bool HasEncoding()
        {
            return Encoding != null; 
        }
    }

    public class InternalCompression
    {
        public String Path;
        public int CompressionVersion;

        public InternalCompression(XmlNode CompressionNode)
        {
            Path = GetPath(CompressionNode);
            CompressionVersion = GetCompressionVersion(CompressionNode);
        }

        private String GetPath(XmlNode CompressionNode)
        {
            return CompressionNode.Attributes["Path"].Value; 
        }

        private int GetCompressionVersion(XmlNode CompressionNode)
        {
            int version = 1;
            if (CompressionNode.Attributes["CompressionVersion"] != null)
            {
                version = Int32.Parse(CompressionNode.Attributes["CompressionVersion"].Value);
            }
            else
            {
                Console.WriteLine("Your interpreter should specify a version for internal FileDBs. For this conversion, 1 was auto-chosen. Make sure your versions match up!");
            }
            return version; 
        }
    }
}

