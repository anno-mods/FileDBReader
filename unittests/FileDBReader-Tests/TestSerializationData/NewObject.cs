using AnnoMods.BBDom.EncodingAwareStrings;
using AnnoMods.ObjectSerializer;
using System;
using System.Collections.Generic;

namespace FileDBReader_Tests.TestSerializationData
{
    public class NewObject
    {
        public int? PrimitiveObject { get; set; }
        public AnyChild? RefObject { get; set; }
        public (int, (int, (int, int))) Tuple { get; set; }

        public int[] PrimitiveArray { get; set; }
        public String[] StringArray { get; set; }
        public ArrayElement[] RefArray { get; set; }
        public (byte, byte)[] TupleArray { get; set; }

        public List<int> IntList { get; set; }
        public List<String> StringList { get; set; }
        public List<AnyChild> ReferenceList { get; set; }
        public List<(byte, AnyChild)> TupleList { get; set; }
        public List<(String, int)> AnotherTupleList { get; set; }
        [FlatArray]
        public List<AnyChild> FlatList { get; set; }

        public List<List<AnyChild>> NestedList { get; set; }
    }

    public class AnyChild
    {
        public String DefaultStr { get; set; }
        public UTF32String EncAwareStr {get; set;}
    }

    public class CountsElement
    {
        public int size { get; set; }
        [FlatArray]
        public CountsElementEntry[] None { get; set; }
    }

    public class CountsElementEntry
    { 
        public (byte, byte) None { get; set; }  
    }

    public class ArrayElement
    {
        public uint ElementContent { get; set; } = 5;
    }
}
