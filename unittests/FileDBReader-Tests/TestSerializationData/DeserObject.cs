using AnnoMods.BBDom.EncodingAwareStrings;
using AnnoMods.ObjectSerializer;
using System;
using System.Collections.Generic;

namespace FileDBReader_Tests.TestSerializationData
{
    public class DeserObject
    {
        public int? IntValue { get; set; }
        public DeserChild? Reference { get; set; }
        public int[] IntArray { get; set; }
        public DeserChild[] ReferenceArray { get; set; }
        public List<DeserChild> ReferenceList { get; set; }
        public List<short> PrimitiveList { get; set; }

        public Tuple<int, DeserChild> IndexTuple { get; set; }
        public Tuple<int, Tuple<short[], string, DeserChild>> NestedTuple { get; set; }

        public List<Tuple<int, DeserChild, DeserChild>> ListOfTuples { get; set; }
        public Tuple<int, DeserChild, DeserChild>[] ArrayOfTuples { get; set; }

        [FlatArray]
        public List<long> FlatLongList { get; set; }
        [FlatArray]
        public List<Tuple<int, DeserChild, DeserChild>> FlatTupleList { get; set; }

        public UnicodeString EncodingAwareString { get; set; }
        public String DefaultString { get; set; }
    }

    public class DeserChild
    {
        public uint? ID { get; set; }
    }
}
