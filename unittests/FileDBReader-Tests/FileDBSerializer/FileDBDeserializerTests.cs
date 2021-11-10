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
            var expected = buildDocument<FileDBDocument_V2>();

            FileDBDeserializer<FileDBDocument_V2> deserializer = new FileDBDeserializer<FileDBDocument_V2>();

            FileDBDocument deser;
            using (Stream s = File.OpenRead("FileDBSerializer/Testfiles/testResult_v2.bin"))
            {
                deser = deserializer.Deserialize(s);
            }

            for (int i = 0; i < deser.Roots.Count && i < expected.Roots.Count; i++)
            {
                AreEqual(deser.Roots.ElementAt(i), expected.Roots.ElementAt(i));
            }
        }

        [TestMethod()]
        public void DeserializeTest_v1()
        {
            var expected = buildDocument<FileDBDocument_V1>();

            FileDBDeserializer<FileDBDocument_V1> deserializer = new FileDBDeserializer<FileDBDocument_V1>();

            FileDBDocument deser;
            using (Stream s = File.OpenRead("FileDBSerializer/Testfiles/testResult_v1.bin"))
            {
                deser = deserializer.Deserialize(s);
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

            FileDBSerializer ser = new FileDBSerializer();

            var docV1 = buildDocument<FileDBDocument_V1>();
            var docstream = new MemoryStream();
            var result = ser.Serialize(docV1, docstream);

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, result));
        }

        [TestMethod]
        public void SerializeTest_v2()
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/testResult_v2.bin");

            FileDBSerializer ser = new FileDBSerializer();

            var docV2 = buildDocument<FileDBDocument_V2>();
            var docstream = new MemoryStream();
            var result = ser.Serialize(docV2, docstream);

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

        internal static FileDBDocument buildDocument<T>() where T : FileDBDocument, new()
        { 
            var filedb = new T();

            //lets create a few tags. 

            Tag root1 = filedb.AddTag("TestRootOne");

            var Attr = filedb.AddAttrib("FloatAttrib_Child");
            Attr.Content = BitConverter.GetBytes(33.2f);
            root1.AddChild(Attr);

            var child1 = filedb.AddTag("None");
            root1.AddChild(child1);

            var root2 = filedb.AddAttrib("None");
            root2.Content = new UnicodeEncoding().GetBytes("Modders are gonna take over the world");

            var root3 = filedb.AddAttrib("StringAttrib_Root");
            root3.Content = new UTF32Encoding().GetBytes("Anno 1800 will release on Stadia, Xbox and PS5. Get ready to open your wallets for Season 4.");

            filedb.Roots.Add(root1);
            filedb.Roots.Add(root2);
            filedb.Roots.Add(root3);
            return filedb;
        }
    }
}