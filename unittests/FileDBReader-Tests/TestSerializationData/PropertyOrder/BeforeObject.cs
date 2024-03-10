using FileDBSerializer.ObjectSerializer;
using AnnoMods.Tests.TestData;
using System.Collections.Generic;

namespace FileDBReader_Tests.TestSerializationData.PropertyOrder
{
    [PropertyLocation(PropertyLocationOption.BEFORE_PARENT)]
    public class BeforeObject : BaseObject
    {
        public long? BeforeID { get; set; }
        public List<ChildElement>? BeforeList { get; set; }
    }
}
