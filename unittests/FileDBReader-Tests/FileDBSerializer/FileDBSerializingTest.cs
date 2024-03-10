using AnnoMods.BBDom;
using AnnoMods.BBDom.IO;
using FileDBReader_Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace AnnoMods.Tests
{
    [TestClass]
    public class FileDBSerializingTest
    {
        [TestMethod]
        public void DeserializeTest_v1()
        {
            var expected = TestDataSources.BuildDocument();
            BBDocumentParser parser = new BBDocumentParser(BBDocumentVersion.V1);

            BBDocument doc;
            using (Stream s = File.OpenRead("FileDBSerializer/Testfiles/testResult_v1.bin"))
            {
                doc = parser.LoadBBDocument(s);
            }

            for (int i = 0; i < doc.Roots.Count && i < expected.Roots.Count; i++)
            {
                AreEqual(doc.Roots.ElementAt(i), expected.Roots.ElementAt(i));
            }
        }

        [TestMethod]
        public void DeserializeTest_v2()
        {
            var expected = TestDataSources.BuildDocument();
            BBDocumentParser parser = new BBDocumentParser(BBDocumentVersion.V2);

            BBDocument doc;
            using (Stream s = File.OpenRead("FileDBSerializer/Testfiles/testResult_v2.bin"))
            {
                doc = parser.LoadBBDocument(s);
            }

            for (int i = 0; i < doc.Roots.Count && i < expected.Roots.Count; i++)
            {
                AreEqual(doc.Roots.ElementAt(i), expected.Roots.ElementAt(i));
            }
        }
        [TestMethod]
        public void DeserializeTest_v3()
        {
            var expected = TestDataSources.BuildDocument();
            BBDocumentParser parser = new BBDocumentParser(BBDocumentVersion.V3);

            BBDocument doc;
            using (Stream s = File.OpenRead("FileDBSerializer/Testfiles/testResult_v3.bin"))
            {
                doc = parser.LoadBBDocument(s);
            }

            for (int i = 0; i < doc.Roots.Count && i < expected.Roots.Count; i++)
            {
                AreEqual(doc.Roots.ElementAt(i), expected.Roots.ElementAt(i));
            }
        }

        [TestMethod]
        public void SerializeTest_v1()
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/testResult_v1.bin");

            BBDocumentWriter ser = new BBDocumentWriter(BBDocumentVersion.V1);

            var docV1 = TestDataSources.BuildDocument();
            var docstream = new MemoryStream();
            var result = ser.WriteToStream(docV1, docstream);

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, result));
        }

        [TestMethod]
        public void SerializeTest_v2()
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/testResult_v2.bin");

            BBDocumentWriter ser = new BBDocumentWriter(BBDocumentVersion.V2);

            var docV2 = TestDataSources.BuildDocument();
            var docstream = new MemoryStream();
            var result = ser.WriteToStream(docV2, docstream);

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, result));
        }

        [TestMethod]
        public void SerializeTest_v3()
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/testResult_v3.bin");

            BBDocumentWriter ser = new BBDocumentWriter(BBDocumentVersion.V3);

            var docV2 = TestDataSources.BuildDocument();
            var docstream = new MemoryStream();
            var result = ser.WriteToStream(docV2, docstream);

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, result));
        }

        public static void AreEqual(BBNode A, BBNode B)
        {
            Assert.AreEqual (A.ID, B.ID);
            Assert.AreEqual(A.NodeType, B.NodeType);
            Assert.IsTrue((A is Attrib && B is Attrib) || (A is Tag && B is Tag));

            if (A is Attrib && B is Attrib)
            {
                CollectionAssert.AreEqual(((Attrib)A).Content, ((Attrib)B).Content);
            }

            else if (A is Tag && B is Tag)
            {
                Assert.AreEqual(((Tag)A).ChildCount, ((Tag)B).ChildCount);
                for(int i = 0; i < ((Tag)A).ChildCount && i < ((Tag)B).ChildCount; i++)
                {
                    AreEqual(((Tag)A).Children.ElementAt(i), ((Tag)B).Children.ElementAt(i));
                }
            }
        }

        
    }
}