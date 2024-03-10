using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace FileDBReader.src
{
    public enum ContentStructure {Default, List, Cdata};

    /// <summary>
    /// Represents an InterpreterFile
    /// </summary>
    public record Interpreter
    {
        //Attribs
        public List<InternalCompression> InternalCompressions = new List<InternalCompression>();
        public List<(String, Conversion)> Conversions = new List<(String, Conversion)>();
        public Conversion DefaultType;

        private static XmlDocument ToInterpreterDoc(Stream InterpreterStream)
        {
            XmlDocument doc = new XmlDocument();
            try
            { 
                doc.Load(InterpreterStream);
                return doc;
            }
            catch (Exception e)
            {
                Console.WriteLine("[INTERPRETER]: Could not load Interpreter from Stream: {0}. Proceeding without an Interpreter.", e.Message);
                //return an empty interpreter document.
                doc.LoadXml("<Converts></Converts>");
                return doc;
            }
        }

        public static Interpreter LoadFrom(Stream InterpreterStream)
        {
            return new Interpreter(ToInterpreterDoc(InterpreterStream));
        }

        public static Interpreter LoadFromFile(String InterpreterPath) 
        {
            using var fs = File.OpenRead(InterpreterPath);
            return LoadFrom(fs);
        }

        public static Interpreter LoadXml(String xmlString)
        {
            XmlDocument doc = new XmlDocument(); 
            doc.LoadXml(xmlString);
            return new Interpreter(doc);
        }

        public String GetCombinedXPath()
        {
            List<String> StringList = new List<String>();
            foreach ((String path, Conversion conv) in Conversions)
                StringList.Add(path);
            foreach (InternalCompression internalFileDB in InternalCompressions)
                StringList.Add(internalFileDB.Path);
            return String.Join(" | ", StringList);
        }

        public void AddConversion(XmlNode Convert)
        {
            try
            {
                Conversion c = new Conversion(Convert);
                String Path = GetPath(Convert);
                Conversions.Add((Path, c));
            }
            catch (XPathException e)
            {
                Console.WriteLine("[INTERPRETER]: Faulty Xpath Detected!");
            }
            catch
            {
                Console.WriteLine("[INTERPRETER]: Faulty Conversion detected!");
            }
        }

        public void AddInternalCompression(XmlNode InternalCompression)
        {
            try
            {
                InternalCompression Compression = new InternalCompression(InternalCompression);
                InternalCompressions.Add(Compression);
            }
            catch
            {
                Console.WriteLine("[INTERPRETER]: Faulty Internal Compression!");
            }
        }

        public void SetDefaultConversion(XmlNode DefaultAttribNode)
        {
            try {
                DefaultType = new Conversion(DefaultAttribNode);
            }
            catch
            {
                Console.WriteLine("[INTERPRETER]: Faulty Default Conversion!");
            }
        }

        public Interpreter() { }

        private Interpreter(XmlDocument InterpreterDoc)
        {
            XmlNode DefaultAttribNode = null;
            DefaultAttribNode = InterpreterDoc.SelectSingleNode("/Converts/Default");
            var ConvertNodes = InterpreterDoc.SelectNodes("/Converts/Converts/Convert");
            var InternalCompressionNodes = InterpreterDoc.SelectNodes("/Converts/InternalCompression/Element");

            //Conversions
            foreach (XmlNode Convert in ConvertNodes)
            {
                AddConversion(Convert);
            }
            //Internal Compression Parsing
            foreach (XmlNode InternalCompression in InternalCompressionNodes)
            {
                AddInternalCompression(InternalCompression);
            }
            //default type
            if (DefaultAttribNode != null)
            {
                SetDefaultConversion(DefaultAttribNode);
            }
        }

        public bool HasDefaultType()
        {
            return DefaultType != null; 
        }

        public String GetPath(XmlNode ConvertNode)
        {
            String Path = ConvertNode.Attributes["Path"].Value;

            //this will throw an error that is caught later on if our xpath is not correct.
            XPathExpression.Compile(Path);

            return Path;
        }
    }

    public record Conversion
    {
        public Type Type;
        public ContentStructure Structure;
        public RuntimeEnum Enum = new RuntimeEnum();
        public Encoding Encoding = new UnicodeEncoding();

        public Conversion()
        { 
        
        }

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

            if (Type is null) throw new Exception();
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
                            Enum.AddValue(Value.Value, Name.Value);
                        }
                        else
                        {
                            Console.WriteLine("[ENUM CONVERSION]: An XML Node Enum Entry was not defined correctly. Please check your interpreter file whether every EnumEntry has an ID and a Name");
                        }
                    }
                    catch (NullReferenceException)
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

    public record InternalCompression
    {
        public String Path;
        public int CompressionVersion;
        public Dictionary<string, string> ReplacementOps = new Dictionary<string, string>();

        public InternalCompression()
        { 
        
        }

        public InternalCompression(XmlNode CompressionNode)
        {
            Path = GetPath(CompressionNode);
            CompressionVersion = GetCompressionVersion(CompressionNode);

            var ReplacementOpsNode = CompressionNode.SelectSingleNode("./ReplaceTagNames");
            if (ReplacementOpsNode is not null)
            {
                ParseReplaceOperations(ReplacementOpsNode);
            }
        }

        private String GetPath(XmlNode CompressionNode)
        {
            return CompressionNode.Attributes["Path"].Value;
        }

        private int GetCompressionVersion(XmlNode CompressionNode)
        {
            int version = 1;
            if (CompressionNode.Attributes["CompressionVersion"] != null)
            version = Int32.Parse(CompressionNode.Attributes["CompressionVersion"].Value);
            return version; 
        }

        private void ParseReplaceOperations(XmlNode ReplacingNode)
        {
            var Entries = ReplacingNode.SelectNodes("./Entry");
            if (Entries != null)
            {
                foreach (XmlNode Entry in Entries)
                {
                    try
                    {
                        var Original = Entry.Attributes["Original"];
                        var Replacement = Entry.Attributes["Replacement"];
                        if (Original != null && Replacement != null)
                        {
                            ReplacementOps.SafeAdd(Original.Value, Replacement.Value);
                        }
                        else
                        {
                            Console.WriteLine("[TAG NAME REPLACEMENT]: An Entry was not defined correctly. Please check your interpreter file whether every Replacement Entry has an Original and a Replacement");
                        }
                    }
                    catch (NullReferenceException)
                    {
                    }
                }
            }
        }
    }
}

