using FileDBReader_Tests;
using FileDBReader_Tests.TestSerializationData;
using FileDBSerializing.ObjectSerializer;
using FileDBSerializing.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

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
    }
}
