using FileDBSerializing.EncodingAwareStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBReader_Tests.TestSerializationData
{
    public class DeserObject
    {
        public int? IntValue { get; set; }
        public DeserChild? Reference { get; set; }
        public int[] IntArray { get; set; }
        public DeserChild[] ReferenceArray { get; set; }

        public UnicodeString EncodingAwareString { get; set; }
        public String DefaultString { get; set; }
    }

    public class DeserChild
    {
        public uint? ID { get; set; }
    }
}
