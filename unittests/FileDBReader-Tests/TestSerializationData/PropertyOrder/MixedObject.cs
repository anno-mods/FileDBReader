using FileDBSerializer.ObjectSerializer;
using AnnoMods.Tests.TestData;
using System.Collections.Generic;

namespace FileDBReader_Tests.TestSerializationData.PropertyOrder
{
    public class MixedObject : BaseObject
    {
        [PropertyLocation(PropertyLocationOption.BEFORE_PARENT)]
        public long? FirstID { get; set; }
        [PropertyLocation(PropertyLocationOption.AFTER_PARENT)]
        public List<ChildElement>? EndList { get; set; }
        public int[]? SecondIntArr { get; set; } //Default to Before, but is after FirstID in this class
    }
}
