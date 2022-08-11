using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileDBSerializing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FileDBReader_Tests;

namespace FileDBSerializing.Tests
{
    [TestClass()]
    public class FileDBSerializingTest
    {
        [TestMethod()]
        public void DeserializeTest_v2()
        {
            var expected = TestDataSources.BuildDocument<FileDBDocument_V2>();

            DocumentParser deserializer = new DocumentParser(FileDBDocumentVersion.Version2);

            IFileDBDocument deser;
            using (Stream s = File.OpenRead("FileDBSerializer/Testfiles/testResult_v2.bin"))
            {
                deser = deserializer.LoadFileDBDocument(s);
            }

            for (int i = 0; i < deser.Roots.Count && i < expected.Roots.Count; i++)
            {
                AreEqual(deser.Roots.ElementAt(i), expected.Roots.ElementAt(i));
            }
        }

        [TestMethod()]
        public void DeserializeTest_v1()
        {
            var expected = TestDataSources.BuildDocument<FileDBDocument_V1>();

            DocumentParser deserializer = new DocumentParser(FileDBDocumentVersion.Version1);

            IFileDBDocument deser;
            using (Stream s = File.OpenRead("FileDBSerializer/Testfiles/testResult_v1.bin"))
            {
                deser = deserializer.LoadFileDBDocument(s);
            }

            for (int i = 0; i < deser.Roots.Count && i < expected.Roots.Count; i++)
            {
                AreEqual(deser.Roots.ElementAt(i), expected.Roots.ElementAt(i));
            }

        }

        [TestMethod]
        public void SerializeTest_v1()
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/testResult_v1.bin");

            DocumentWriter ser = new DocumentWriter();

            var docV1 = TestDataSources.BuildDocument<FileDBDocument_V1>();
            var docstream = new MemoryStream();
            var result = ser.WriteFileDBToStream(docV1, docstream);

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, result));
        }

        [TestMethod]
        public void SerializeTest_v2()
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/testResult_v2.bin");

            DocumentWriter ser = new DocumentWriter();

            var docV2 = TestDataSources.BuildDocument<FileDBDocument_V2>();
            var docstream = new MemoryStream();
            var result = ser.WriteFileDBToStream(docV2, docstream);

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, result));
        }

        public static void AreEqual(FileDBNode A, FileDBNode B)
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