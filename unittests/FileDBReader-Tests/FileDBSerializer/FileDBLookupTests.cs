using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileDBSerializing;
using FileDBReader_Tests;

namespace FileDBSerializing.Tests
{
    [TestClass]
    public class FileDBLookupTests
    {
        [TestMethod]
        public void LookUpTest_Attribute()
        {
            FileDBDocument document = TestDataSources.BuildDocument<FileDBDocument_V2>();

            Attrib floatattr = (Attrib)document.SelectNodes("TestRootOne/FloatAttrib_Child").First();

            Attrib a = new Attrib() { Content = BitConverter.GetBytes(33.2f)};
            CollectionAssert.AreEquivalent(a.Content, floatattr.Content);
        }

        [TestMethod]
        public void LookUpTest_Tag()
        {
            FileDBDocument document = TestDataSources.BuildDocument<FileDBDocument_V2>();

            Tag Tag = (Tag)document.SelectNodes("TestRootOne/None").First();

            var TagName = "None";
            Assert.AreEqual(TagName, Tag.GetName());
        }
    }
}
