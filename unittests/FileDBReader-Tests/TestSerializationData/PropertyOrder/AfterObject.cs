using AnnoMods.BBDom.ObjectSerializer;
using AnnoMods.Tests.TestData;
using System.Collections.Generic;

namespace FileDBReader_Tests.TestSerializationData.PropertyOrder
{
    [PropertyLocation(PropertyLocationOption.AFTER_PARENT)]
    public class AfterObject : BaseObject
    {
        public long? AfterID { get; set; }
        public List<ChildElement>? AfterList { get; set; }
    }
}
