using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using FileDBSerializing.EncodingAwareStrings;
using FileDBSerializing.ObjectSerializer;

namespace FileDBSerializing.Tests.TestData
{
    public record RootObject
    {
        public int? RootCount { get; set; }
        public SomethingManager? DumbManager { get; set; }
        public ChildElement? DumbChild { get; set; }
        public int[]? PrimitiveArray { get; set; }
        public List<ChildElement>? RefList { get; set; }
        public UnicodeString? SimpleString { get; set; }
        public List<String>? StringList { get; set; }
        public ChildElement[] RefArray { get; set; }
    }

    public record SomethingManager
    {
        public int? SomethingCount { get; set; }
        public float? SomethingValue { get; set; }
        public ChildElement? Child { get; set; }
    }

    public record ChildElement
    {
        public uint? ID { get; set; }
    }

    public record RenamedRootObject
    {
        [RenameProperty("RootCount")]
        public int? UnknownCount { get; set; }

        [RenameProperty("DumbManager")]
        public SomethingManager? IntelligentManager { get; set; }
        public ChildElement? DumbChild { get; set; }

        [RenameProperty("PrimitiveArray")]
        public int[]? IntArray { get; set; }

        [RenameProperty("RefList")]
        public List<ChildElement>? RefNotAList { get; set; }
        public UnicodeString? SimpleString { get; set; }

        [RenameProperty("StringList")]
        public List<String>? StringNotAList { get; set; }

        [RenameProperty("RefArray")]
        public ChildElement[]? ChildArray { get; set; }
    }
}
