using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileDBReader_Tests;
using FileDBSerializing.ObjectSerializer;
using FileDBSerializing.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileDBSerializing.Tests
{
    [TestClass]
    public class ObjectSerializerTest
    {
        [TestMethod]
        public void SerializeTest_V1() 
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version1.filedb");

            var obj = TestDataSources.GetTestAsset();
            FileDBDocumentBuilder<RootObject> objectserializer = new FileDBDocumentBuilder<RootObject>(FileDBDocumentVersion.Version1);
            Stream result = new MemoryStream();
            objectserializer.Serialize(result, obj);

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, result));
        }

        public void SerializeTest_V2()
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version2.filedb");

            var obj = TestDataSources.GetTestAsset();
            FileDBDocumentBuilder<RootObject> objectserializer = new FileDBDocumentBuilder<RootObject>(FileDBDocumentVersion.Version2);
            Stream result = new MemoryStream();
            objectserializer.Serialize(result, obj);

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, result));
        }
    }
}
