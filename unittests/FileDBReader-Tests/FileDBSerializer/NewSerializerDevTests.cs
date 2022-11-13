using FileDBReader_Tests;
using FileDBReader_Tests.TestSerializationData;
using FileDBSerializing.ObjectSerializer;
using FileDBSerializing.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

using FluentAssertions;

namespace FileDBSerializing.Tests
{
    [TestClass]
    public class NewSerializerDevTests
    {
        [TestMethod]
        public void NewObjectTest()
        {
            var obj = TestDataSources.GetSmallTestAsset();
            FileDBSerializer<NewObject> objectserializer = new FileDBSerializer<NewObject>(FileDBDocumentVersion.Version2);
            Stream result = File.Create("myfile.filedb");
            objectserializer.Serialize(result, obj);
        }

        [TestMethod]
        public void NewDeserializerTest()
        {
            var obj = TestDataSources.GetDeserializerTestAsset();
            FileDBSerializer<DeserObject> serializer = new(FileDBDocumentVersion.Version2);
            using (Stream result = File.Create("deser.filedb"))
            {
                serializer.Serialize(result, obj);
            }
            DeserObject? deser;
            using (var instream = File.OpenRead("deser.filedb"))
            {
                deser = serializer.Deserialize(instream) as DeserObject;
            }
            deser.Should().BeEquivalentTo(obj);
        }
    }
}
