using FileDBReader_Tests;
using FileDBReader_Tests.TestSerializationData;
using AnnoMods.BBDom.ObjectSerializer;
using AnnoMods.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

using FluentAssertions;
using AnnoMods.BBDom;

namespace AnnoMods.Tests
{
    [TestClass]
    public class NewSerializerDevTests
    {
        [TestMethod]
        public void NewObjectTest()
        {
            var obj = TestDataSources.GetSmallTestAsset();
            BBSerializer<NewObject> objectserializer = new BBSerializer<NewObject>(BBDocumentVersion.V2);
            Stream result = File.Create("myfile.filedb");
            objectserializer.Serialize(result, obj);
        }

        [TestMethod]
        public void NewDeserializerTest()
        {
            var obj = TestDataSources.GetDeserializerTestAsset();
            BBSerializer<DeserObject> serializer = new(BBDocumentVersion.V2);
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
