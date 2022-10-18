using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using FileDBSerializing.EncodingAwareStrings;

namespace FileDBSerializing.Tests.TestData
{
    public record RootObject
    {
        public int? RootCount { get; set; }
        public SomethingManager? DumbManager { get; set; }
        public ChildElement? DumbChild { get; set; }
        public int[]? PrimitiveArray { get; set; }
        public List<ChildElement>? RefArray { get; set; } = new();
        public UnicodeString? SimpleString { get; set; }
        public List<String>? StringArray { get; set; } = new();
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
}
