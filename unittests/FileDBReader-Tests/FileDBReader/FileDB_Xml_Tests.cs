using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileDBSerializing;
using FileDBSerializing.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileDBReader.src.XmlRepresentation;
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
            var workDoc = TestDataSources.BuildDocument<FileDBDocument_V1>();

            var Expected = new XmlDocument();
            Expected.Load(expectedPath);

            FileDbXmlConverter ser = new FileDbXmlConverter();
            var x = ser.ToXml(workDoc);

            XmlDocumentEqual(Expected, x);
        }

        [TestMethod]
        private void FileDBToXmlTest_v2()
        {
            var Expected = TestDataSources.BuildDocument<FileDBDocument_V2>();

            XmlFileDbConverter serial = new(FileDBDocumentVersion.Version2);
            var workDoc = new XmlDocument();
            workDoc.Load(expectedPath);

            var fromXml = serial.ToFileDb(workDoc);

            Assert.IsTrue(fromXml.ELEMENT_COUNT == Expected.ELEMENT_COUNT);
            for (int i = 0; i < fromXml.ELEMENT_COUNT && i < Expected.ELEMENT_COUNT; i++)
            {
                FileDBSerializingTest.AreEqual(fromXml.Roots.ElementAt(i), Expected.Roots.ElementAt(i));
            }
        }

        [TestMethod]
        private void FileDBToXmlTest_v1()
        {
            var Expected = TestDataSources.BuildDocument<FileDBDocument_V1>();

            XmlFileDbConverter serial = new(FileDBDocumentVersion.Version1);
            var workDoc = new XmlDocument();
            workDoc.Load(expectedPath);

            var fromXml = serial.ToFileDb(workDoc);

            Assert.IsTrue(fromXml.ELEMENT_COUNT == Expected.ELEMENT_COUNT);
            for (int i = 0; i < fromXml.ELEMENT_COUNT && i < Expected.ELEMENT_COUNT; i++)
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
