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
    //todo (taubenangriff) Add some bigger tests for node lookups.
    public class FileDBLookupTests
    {
        [TestMethod]
        public void LookUpTest_Attribute()
        {
            IFileDBDocument document = TestDataSources.BuildDocument<FileDBDocument_V2>();
            Attrib floatattr = (Attrib)document.SelectNodes("TestRootOne/FloatAttrib_Child").First();
            Attrib a = new Attrib() { Content = BitConverter.GetBytes(33.2f)};
            CollectionAssert.AreEquivalent(a.Content, floatattr.Content);
        }

        [TestMethod]
        public void LookUpTest_Tag()
        {
            IFileDBDocument document = TestDataSources.BuildDocument<FileDBDocument_V2>();
            Tag Tag = (Tag)document.SelectNodes("TestRootOne/None").First();
            var TagName = "None";
            Assert.AreEqual(TagName, Tag.GetName());
        }

        [TestMethod]
        public void LookUpTest_Attrib_Single()
        { 
            IFileDBDocument document = TestDataSources.BuildDocument<FileDBDocument_V2>();
            Attrib floatattr = (Attrib)document.SelectSingleNode("TestRootOne/FloatAttrib_Child");
            var bytes = BitConverter.GetBytes(33.2f);
            CollectionAssert.AreEquivalent(bytes, floatattr.Content);
        }

        [TestMethod]
        public void LookUpTest_Tag_Single()
        {
            IFileDBDocument document = TestDataSources.BuildDocument<FileDBDocument_V2>();
            Tag Tag = (Tag)document.SelectSingleNode("TestRootOne/None");
            var TagName = "None";
            Assert.AreEqual(TagName, Tag.GetName());
        }
    }
}
