using AnnoMods.BBDom.XML;
using AnnoMods.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml;

namespace FileDBReader_Tests
{
    [TestClass]
    public class FileDB_Xml_Tests
    {
        String expectedPath = "unittest_files/Expected/xmlserializing/xmlresult.xml";

        [TestMethod]
        //this does not need two versions because I am just comparing xml to object structure (which is sorta the same for both versions).
        public void XmlToFiledbTest()
        {
            var workDoc = TestDataSources.BuildDocument();
            var Expected = new XmlDocument();
            Expected.Load(expectedPath);

            var x = workDoc.ToXmlDocument(); 
            XmlDocumentEqual(Expected, x);
        }

        [TestMethod]
        private void FileDBToXmlTest_v2()
        {
            var Expected = TestDataSources.BuildDocument();

            var workDoc = new XmlDocument();
            workDoc.Load(expectedPath);

            var fromXml = workDoc.ToBBDocument();

            Assert.IsTrue(fromXml.ElementCount == Expected.ElementCount);
            for (int i = 0; i < fromXml.ElementCount && i < Expected.ElementCount; i++)
            {
                FileDBSerializingTest.AreEqual(fromXml.Roots.ElementAt(i), Expected.Roots.ElementAt(i));
            }
        }

        [TestMethod]
        private void FileDBToXmlTest_v1()
        {
            var Expected = TestDataSources.BuildDocument();

            var workDoc = new XmlDocument();
            workDoc.Load(expectedPath);
            var fromXml = workDoc.ToBBDocument();

            Assert.IsTrue(fromXml.ElementCount == Expected.ElementCount);
            for (int i = 0; i < fromXml.ElementCount && i < Expected.ElementCount; i++)
            {
                FileDBSerializingTest.AreEqual(fromXml.Roots.ElementAt(i), Expected.Roots.ElementAt(i));
            }
        }

        private void XmlDocumentEqual(XmlDocument A, XmlDocument B)
        {
            XmlNodeEqual(A.FirstChild, B.FirstChild);
        }


        private void XmlNodeEqual(XmlNode? A, XmlNode? B)
        {
            if (A.ChildNodes.Count == 0 && B.ChildNodes.Count == 0)
            {
                Assert.IsTrue(A.InnerText.Equals(B.InnerText) && A.Name.Equals(B.Name));
            }
            else if (A.ChildNodes.Count > 0 || B.ChildNodes.Count > 0)
            {
                //compare children
                Assert.IsTrue(A.ChildNodes.Count == B.ChildNodes.Count);
                for (int i = 0; i < A.ChildNodes.Count && i < B.ChildNodes.Count; i++)
                {
                    XmlNodeEqual(A.ChildNodes.Item(i), B.ChildNodes.Item(i));
                }
            }
            else Assert.Fail();
        }

    }

    
}
