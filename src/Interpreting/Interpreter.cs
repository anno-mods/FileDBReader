using System;
using System.Collections.Generic;
using System.Xml;

namespace FileDBReader.src
{
    enum ContentStructure {Default, List, CDATA};
    public class Interpreter
    {
        public IEnumerable<String> InternalCompressions;
        public Dictionary<string, Conversion> Conversions;
        public Conversion DefaultType;
    }
    public Interpreter()
    {
        
    }

    public Interpreter(String InterpreterPath)
    {
        XmlDocument doc = new XmlDocument(); 
        try
        {
            doc.Load(InterpreterPath);
        }
        catch (XmlException e)
        {
            Console.WriteLine("Could not load Interpreter at: {0}", InterpreterPath);
            this = null;
            return; 
        }
    }

    private RuntimeEnum ToEnum(XmlNode EnumNode)
    {
        //getEnum
        var EnumEntries = ConverterInfo.SelectNodes("./Entry");
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

    private Conversion ToConversion(XmlNode ConvertNode)
    {
        //gather info from the Conversion Attributes
        var PathAttr = ConvertNode.Attributes["Path"];
        var TypeAttr = ConvertNode.Attributes["Type"];
        var StructureAttr = ConvertNode.Attributes["Structure"];
        var EncodingAttr = ConvertNode.Attributes["Encoding"];

    }

    private Type ToType(string typeStr)
    {
        return Type.Get("System." + typeStr);
    }

    private ContentStructure ToContentStructure(string structureStr)
    {
        if (Enum.IsDefined<ContentStructure>(structureStr))
            return Enum.Parse<ContentStructure>(structureStr);
        else
            return ContentStructure.Default; 
    }

    private Encoding ToEncoding(string encodingStr)
    {
        return Encoding.GetEncoding(encodingStr);
    }

    public class Conversion
    {
        public Type Type;
        public ContentStructure Structure;
        public RuntimeEnum Enum;
        public Encoding Encoding;

        public bool HasPath()
        {
            return Path != null; 
        }

        public bool hasType()
        {
            return Type != null;         
        }

        public bool hasStructure()
        {
            return Structure != null; 
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
}

