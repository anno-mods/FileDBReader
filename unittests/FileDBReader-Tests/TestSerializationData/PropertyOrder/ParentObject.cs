namespace FileDBReader_Tests.TestSerializationData.PropertyOrder
{
    public class ParentObject
    {
        public BeforeObject? Before { get; set; }
        public AfterObject? After { get; set; }
        public MixedObject? Mixed { get; set; }
    }
}
