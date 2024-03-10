using AnnoMods.Tests.TestData;

namespace FileDBReader_Tests.TestSerializationData.PropertyOrder
{
    public class BaseObject
    {
        public int? BaseCount { get; set; }
        public ChildElement? BaseChild { get; set; }
    }
}
