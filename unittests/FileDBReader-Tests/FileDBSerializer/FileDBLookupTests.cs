using AnnoMods.BBDom;
using AnnoMods.BBDom.LookUps;
using FileDBReader_Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AnnoMods.Tests
{
    [TestClass]
    //todo (taubenangriff) Add some bigger tests for node lookups.
    public class FileDBLookupTests
    {
        [TestMethod]
        public void LookUpTest_Attribute()
        {
            BBDocument document = TestDataSources.BuildDocument();
            Attrib floatattr = (Attrib)document.SelectNodes("TestRootOne/FloatAttrib_Child").First();
            var content = BitConverter.GetBytes(33.2f); 
            CollectionAssert.AreEquivalent(content, floatattr.Content);
        }

        [TestMethod]
        public void LookUpTest_Tag()
        {
            BBDocument document = TestDataSources.BuildDocument();
            Tag Tag = (Tag)document.SelectNodes("TestRootOne/None").First();
            var TagName = "None";
            Assert.AreEqual(TagName, Tag.GetName());
        }

        [TestMethod]
        public void LookUpTest_Attrib_Single()
        { 
            BBDocument document = TestDataSources.BuildDocument();
            Attrib? floatattr = document.SelectSingleNode("TestRootOne/FloatAttrib_Child") as Attrib;
            var bytes = BitConverter.GetBytes(33.2f);
            CollectionAssert.AreEquivalent(bytes, floatattr?.Content);
        }

        [TestMethod]
        public void LookUpTest_Tag_Single()
        {
            BBDocument document = TestDataSources.BuildDocument();
            Tag? Tag = document.SelectSingleNode("TestRootOne/None") as Tag;
            var TagName = "None";
            Assert.AreEqual(TagName, Tag?.GetName());
        }
    }
}
