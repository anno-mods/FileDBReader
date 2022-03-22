using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using FileDBSerializer.EncodingAwareStrings;

namespace FileDBSerializing.Tests.TestData
{
    public record RootObject
    {
        public int RootCount { get; set; }
        public SomethingManager DumbManager { get; set; }
        public ChildElement DumbChild { get; set; }
        public int[] PrimitiveArray { get; set; }
        public ChildElement[] RefArray { get; set; }
        public UnicodeString SimpleString { get; set; }
        public String[] StringArray { get; set; }
    }

    public record SomethingManager
    {
        public int SomethingCount { get; set; }
        public float SomethingValue { get; set; }
        public ChildElement Child { get; set; }
    }

    public record ChildElement
    {
        public uint ID { get; set; }
    }
}
