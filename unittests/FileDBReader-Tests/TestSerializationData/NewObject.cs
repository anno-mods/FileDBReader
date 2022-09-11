using FileDBSerializing.EncodingAwareStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBReader_Tests.TestSerializationData
{
    public class NewObject
    {
        public int? Count { get; set; }
        public float? Value { get; set; }
        public AnyChild? Child { get; set; }

        public bool IsNew { get; set; }
    }

    public class AnyChild
    {
        public uint? ID { get; set; }
        public String Name { get; set; }
        public UTF32String Description {get; set;}
    }
}
