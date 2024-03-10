using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FileDBReader.src.XmlRepresentation;
using FileDBReader_Tests;
using AnnoMods.ObjectSerializer;
using AnnoMods.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using FileDBReader_Tests.TestSerializationData.PropertyOrder;
using Newtonsoft.Json.Linq;
using AnnoMods.BBDom;
using AnnoMods.BBDom.IO;
using AnnoMods.BBDom.XML;

namespace AnnoMods.Tests
{
    [TestClass]
    public class ObjectSerializerTest
    {
        #region RootObject De-/Serialization Tests

        [TestMethod]
        public void SerializeTest_V1() 
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version1.filedb");

            var obj = TestDataSources.GetTestAsset();
            BBSerializer<RootObject> objectserializer = new BBSerializer<RootObject>(BBDocumentVersion.V1);
            
            Stream result = new MemoryStream();
            objectserializer.Serialize(result, obj);

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, result));
        }

        [TestMethod]
        public void SerializeTest_V2()
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version2.filedb");

            var obj = TestDataSources.GetTestAsset();
            BBSerializer<RootObject> objectserializer = new BBSerializer<RootObject>(BBDocumentVersion.V2);
            MemoryStream result = new MemoryStream();
            objectserializer.Serialize(result, obj);

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, result));
        }

        [TestMethod]
        public void DeserializeTest_V1()
        {
            var x = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version1.filedb");

            BBDocumentParser parser = new BBDocumentParser(BBDocumentVersion.V1);
            BBDocument doc = parser.LoadBBDocument(x);

            BBDocumentDeserializer<RootObject> objectdeserializer = new BBDocumentDeserializer<RootObject>(new() { Version = BBDocumentVersion.V1 });

            var DeserializedDocument = objectdeserializer.GetObjectStructureFromFileDBDocument(doc);

            DeserializedDocument.Should().BeEquivalentTo(TestDataSources.GetTestAsset());
        }

        [TestMethod]
        public void DeserializeTest_V2()
        {
            var x = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version2.filedb");

            BBDocumentParser parser = new BBDocumentParser(BBDocumentVersion.V2);
            BBDocument doc = parser.LoadBBDocument(x);

            BBDocumentDeserializer<RootObject> objectdeserializer = new BBDocumentDeserializer<RootObject>(new() { Version = BBDocumentVersion.V1 });

            var DeserializedDocument = objectdeserializer.GetObjectStructureFromFileDBDocument(doc);

            DeserializedDocument.Should().BeEquivalentTo(TestDataSources.GetTestAsset());
        }

        [TestMethod]
        public void StaticConvertTest_Serialize()
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version2.filedb");

            var obj = TestDataSources.GetTestAsset();

            Stream Result = BBConvert.SerializeObject(obj, new() { Version = BBDocumentVersion.V2 });

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, Result));
        }

        [TestMethod]
        public void StaticConvertTest_Deserialize()
        {
            var source = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version2.filedb");
            RootObject? result = BBConvert.DeserializeObject<RootObject>(source, new() { Version = BBDocumentVersion.V2 });

            result.Should().BeEquivalentTo(TestDataSources.GetTestAsset());
        }

        #endregion

        #region RenamedRootObject De-/Serialization Tests


        [TestMethod]
        public void SerializeTestRenamed_V1()
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version1.filedb");

            var obj = TestDataSources.GetTestAssetRenamed();
            BBSerializer<RenamedRootObject> objectserializer = new BBSerializer<RenamedRootObject>(BBDocumentVersion.V1);

            Stream result = new MemoryStream();
            objectserializer.Serialize(result, obj);

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, result));
        }

        [TestMethod]
        public void SerializeTestRenamed_V2()
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version2.filedb");

            var obj = TestDataSources.GetTestAssetRenamed();
            BBSerializer<RenamedRootObject> objectserializer = new BBSerializer<RenamedRootObject>(BBDocumentVersion.V2);
            MemoryStream result = new MemoryStream();
            objectserializer.Serialize(result, obj);

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, result));
        }



        [TestMethod]
        public void DeserializeTestRenamed_V1()
        {
            var x = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version1.filedb");

            BBDocumentParser parser = new BBDocumentParser(BBDocumentVersion.V1);
            BBDocument doc = parser.LoadBBDocument(x);

            BBDocumentDeserializer<RenamedRootObject> objectdeserializer = new BBDocumentDeserializer<RenamedRootObject>(new() { Version = BBDocumentVersion.V1 });

            var DeserializedDocument = objectdeserializer.GetObjectStructureFromFileDBDocument(doc);

            DeserializedDocument.Should().BeEquivalentTo(TestDataSources.GetTestAssetRenamed());
        }

        [TestMethod]
        public void DeserializeTestRenamed_V2()
        {
            var x = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version2.filedb");

            BBDocumentParser parser = new BBDocumentParser(BBDocumentVersion.V2);
            BBDocument doc = parser.LoadBBDocument(x);

            BBDocumentDeserializer<RenamedRootObject> objectdeserializer = new BBDocumentDeserializer<RenamedRootObject>(new() { Version = BBDocumentVersion.V1 });

            var DeserializedDocument = objectdeserializer.GetObjectStructureFromFileDBDocument(doc);

            DeserializedDocument.Should().BeEquivalentTo(TestDataSources.GetTestAssetRenamed());
        }



        [TestMethod]
        public void StaticConvertTestRenamed_Serialize()
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version2.filedb");

            var obj = TestDataSources.GetTestAssetRenamed();

            Stream Result = BBConvert.SerializeObject(obj, new() { Version = BBDocumentVersion.V2 });

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, Result));
        }

        [TestMethod]
        public void StaticConvertTestRenamed_Deserialize()
        {
            var source = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version2.filedb");
            RenamedRootObject? result = BBConvert.DeserializeObject<RenamedRootObject>(source, new() { Version = BBDocumentVersion.V2 });

            result.Should().BeEquivalentTo(TestDataSources.GetTestAssetRenamed());
        }

        #endregion


        [TestMethod()]
        public void SkipSimpleNullValues()
        {
            // test default setting
            BBSerializerOptions options = new() { Version = BBDocumentVersion.V1 };
            Assert.IsTrue(options.SkipSimpleNullValues); 
            
            // all null
            var obj = new RootObject();
            BBDocumentSerializer serializer = new(new() { Version = BBDocumentVersion.V1 });
            BBDocument doc = serializer.WriteObjectStructureToFileDBDocument(obj);
            XmlDocument xmlDocument = new FileDbXmlConverter().ToXml(doc);
            Assert.AreEqual("<Content />", xmlDocument.InnerXml);

            // all null, SkipReferenceArrayNullValues = false
            serializer = new(new() { Version = BBDocumentVersion.V1, SkipReferenceArrayNullValues = false });
            doc = serializer.WriteObjectStructureToFileDBDocument(obj);
            xmlDocument = new FileDbXmlConverter().ToXml(doc);
            Assert.AreEqual(
                "<Content>" +
                "<RefArray />" +
                "</Content>", xmlDocument.InnerXml);

            // all null, SkipSimpleNullValues = false
            serializer = new(new() { Version = BBDocumentVersion.V1, SkipSimpleNullValues = false });
            doc = serializer.WriteObjectStructureToFileDBDocument(obj);
            xmlDocument = new FileDbXmlConverter().ToXml(doc);
            Assert.AreEqual(
                "<Content>" +
                "<RootCount></RootCount>" + // TODO: why are simple null values not self-closing?
                "<DumbManager />" +
                "<DumbChild />" +
                "<PrimitiveArray></PrimitiveArray>" +
                "<SimpleString></SimpleString>" +
                "</Content>", xmlDocument.InnerXml);

            // all null, SkipSimpleNullValues = false, SkipListNullValues = false
            serializer = new(new() { Version = BBDocumentVersion.V1, SkipSimpleNullValues = false, SkipListNullValues = false });
            doc = serializer.WriteObjectStructureToFileDBDocument(obj);
            xmlDocument = new FileDbXmlConverter().ToXml(doc);
            Assert.AreEqual(
                "<Content>" +
                "<RootCount></RootCount>" + // TODO: why are simple null values not self-closing?
                "<DumbManager />" +
                "<DumbChild />" +
                "<PrimitiveArray></PrimitiveArray>" +
                "<RefList />" +
                "<SimpleString></SimpleString>" +
                "<StringList />" + 
                "</Content>", xmlDocument.InnerXml);


            // all null, SkipSimpleNullValues = false, SkipListNullValues = false, SkipReferenceArrayNullValues = false
            serializer = new(new() { Version = BBDocumentVersion.V1, SkipSimpleNullValues = false, SkipListNullValues = false, SkipReferenceArrayNullValues = false });
            doc = serializer.WriteObjectStructureToFileDBDocument(obj);
            xmlDocument = new FileDbXmlConverter().ToXml(doc);
            Assert.AreEqual(
                "<Content>" +
                "<RootCount></RootCount>" + // TODO: why are simple null values not self-closing?
                "<DumbManager />" +
                "<DumbChild />" +
                "<PrimitiveArray></PrimitiveArray>" +
                "<RefList />" +
                "<SimpleString></SimpleString>" +
                "<StringList />" +
                "<RefArray />" +
                "</Content>", xmlDocument.InnerXml);
        }

        /// <summary>
        /// A Reference Array with only 1 child element is illegal - the size tag should only appear when the array size is &gt;=1.
        /// </summary>
        [TestMethod]
        public void ErrorOnInvalidReferenceArray()
        {
            string invalidReferenceArrayXML =
                "<Content>" +
                "<RefArray>" +
                "<size>00000000</size>" +
                "</RefArray>" +
                "</Content>";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(invalidReferenceArrayXML);

            XmlToBBDocumentConverter xmlFileDbConverter = new XmlToBBDocumentConverter();

            BBDocument doc = xmlFileDbConverter.ToBBDocument(xmlDoc);

            BBDocumentDeserializer<RootObject> objectdeserializer = new BBDocumentDeserializer<RootObject>(new() { Version = BBDocumentVersion.V1 });

            RootObject DeserializedDocument;
            Assert.ThrowsException<InvalidOperationException>(() => DeserializedDocument = objectdeserializer.GetObjectStructureFromFileDBDocument(doc));
        }

        [TestMethod]
        public void EmptyReferenceArrayRoundTrip()
        {
            string emptyReferenceArrayXML =
                "<Content>" +
                "<RefArray />" +
                "</Content>";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(emptyReferenceArrayXML);

            XmlToBBDocumentConverter xmlFileDbConverter = new XmlToBBDocumentConverter();

            BBDocument fromXML = xmlFileDbConverter.ToBBDocument(xmlDoc);
            BBDocumentDeserializer<RootObject> objectdeserializer = new BBDocumentDeserializer<RootObject>(new() { Version = BBDocumentVersion.V1 });
            RootObject? DeserializedDocument = objectdeserializer.GetObjectStructureFromFileDBDocument(fromXML);

            BBDocumentSerializer serializer = new(new() { Version = BBDocumentVersion.V1 });
            BBDocument toXml = serializer.WriteObjectStructureToFileDBDocument(DeserializedDocument);
            XmlDocument xmlDocumentResult = new FileDbXmlConverter().ToXml(toXml);

            Assert.AreEqual(emptyReferenceArrayXML, xmlDocumentResult.InnerXml);
        }

        private class FlatStringArrayContainer
        {
            [FlatArray]
            public List<String>? Item { get; set; } = new();
        }

        [TestMethod]
        public void TestPropertyOrderBefore()
        {
            var obj = new BeforeObject() { BaseCount = 1, BaseChild = new ChildElement() { ID = 1 } };
            obj.BeforeID = 1;
            obj.BeforeList = new List<ChildElement>();

            BBDocumentSerializer serializer = new(new() { Version = BBDocumentVersion.V1 });
            BBDocument doc = serializer.WriteObjectStructureToFileDBDocument(obj);
            XmlDocument xmlDocument = new FileDbXmlConverter().ToXml(doc);

            Assert.AreEqual(
                "<Content>" +
                "<BeforeID>0100000000000000</BeforeID>" +
                "<BeforeList />" +
                "<BaseCount>01000000</BaseCount>" +
                "<BaseChild>" + 
                "<ID>01000000</ID>" +
                "</BaseChild>" +
                "</Content>", 
                xmlDocument.InnerXml);
        }

        [TestMethod]
        public void TestPropertyOrderAfter()
        {
            var obj = new AfterObject() { BaseCount = 2, BaseChild = new ChildElement() { ID = 2 } };
            obj.AfterID = 2;
            obj.AfterList = new List<ChildElement>();

            BBDocumentSerializer serializer = new(new() { Version = BBDocumentVersion.V1 });
            BBDocument doc = serializer.WriteObjectStructureToFileDBDocument(obj);
            XmlDocument xmlDocument = new FileDbXmlConverter().ToXml(doc);

            Assert.AreEqual(
                "<Content>" +
                "<BaseCount>02000000</BaseCount>" +
                "<BaseChild>" +
                "<ID>02000000</ID>" +
                "</BaseChild>" +
                "<AfterID>0200000000000000</AfterID>" +
                "<AfterList />" +
                "</Content>",
                xmlDocument.InnerXml);
        }

        [TestMethod]
        public void TestPropertyOrderMixed()
        {
            var obj = new MixedObject() { BaseCount = 3, BaseChild = new ChildElement() { ID = 3 } };
            obj.FirstID = 3;
            obj.EndList = new List<ChildElement>();
            obj.SecondIntArr = new int[] { 0, 1 };

            BBDocumentSerializer serializer = new(new() { Version = BBDocumentVersion.V1 });
            BBDocument doc = serializer.WriteObjectStructureToFileDBDocument(obj);
            XmlDocument xmlDocument = new FileDbXmlConverter().ToXml(doc);

            Assert.AreEqual(
                "<Content>" +
                "<FirstID>0300000000000000</FirstID>" +
                "<SecondIntArr>0000000001000000</SecondIntArr>" +
                "<BaseCount>03000000</BaseCount>" +
                "<BaseChild>" +
                "<ID>03000000</ID>" +
                "</BaseChild>" +
                "<EndList />" +
                "</Content>",
                xmlDocument.InnerXml);
        }

        [TestMethod]
        public void TestPropertyOrderNested()
        {
            var before = new BeforeObject() { BaseCount = 1, BaseChild = new ChildElement() { ID = 1 } };
            before.BeforeID = 1;
            before.BeforeList = new List<ChildElement>();

            var after = new AfterObject() { BaseCount = 2, BaseChild = new ChildElement() { ID = 2 } };
            after.AfterID = 2;
            after.AfterList = new List<ChildElement>();

            var mixed = new MixedObject() { BaseCount = 3, BaseChild = new ChildElement() { ID = 3 } };
            mixed.FirstID = 3;
            mixed.EndList = new List<ChildElement>();
            mixed.SecondIntArr = new int[] { 0, 1 };

            var obj = new ParentObject() { Before = before, After = after, Mixed = mixed };

            BBDocumentSerializer serializer = new(new() { Version = BBDocumentVersion.V1 });
            BBDocument doc = serializer.WriteObjectStructureToFileDBDocument(obj);
            XmlDocument xmlDocument = new FileDbXmlConverter().ToXml(doc);

            string nestedXML =
                "<Content>" +
                "<Before>" +
                "<BeforeID>0100000000000000</BeforeID>" +
                "<BeforeList />" +
                "<BaseCount>01000000</BaseCount>" +
                "<BaseChild>" +
                "<ID>01000000</ID>" +
                "</BaseChild>" +
                "</Before>" +
                "<After>" +
                "<BaseCount>02000000</BaseCount>" +
                "<BaseChild>" +
                "<ID>02000000</ID>" +
                "</BaseChild>" +
                "<AfterID>0200000000000000</AfterID>" +
                "<AfterList />" +
                "</After>" +
                "<Mixed>" +
                "<FirstID>0300000000000000</FirstID>" +
                "<SecondIntArr>0000000001000000</SecondIntArr>" +
                "<BaseCount>03000000</BaseCount>" +
                "<BaseChild>" +
                "<ID>03000000</ID>" +
                "</BaseChild>" +
                "<EndList />" +
                "</Mixed>" +
                "</Content>";

            Assert.AreEqual(nestedXML, xmlDocument.InnerXml);
        }

        [TestMethod()]
        public void DeSerializeFlatStringArrayNull()
        {
            static Stream stream(string x) => new MemoryStream(Encoding.Unicode.GetBytes(x));

            // load from XML
            XmlDocument xmlDocument = new();
            xmlDocument.Load(stream("<Content></Content>"));
            BBDocument doc = new XmlToBBDocumentConverter().ToBBDocument(xmlDocument);

            // serialize & deserialize
            BBDocumentDeserializer<FlatStringArrayContainer> deserializer = new(new() { Version = BBDocumentVersion.V1 });
            var obj = deserializer.GetObjectStructureFromFileDBDocument(doc);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.Item!.Count == 0);

            BBDocumentSerializer serializer = new(new() { Version = BBDocumentVersion.V1 });
            doc = serializer.WriteObjectStructureToFileDBDocument(obj);

            // convert back to xml
            xmlDocument = new FileDbXmlConverter().ToXml(doc);
            Assert.AreEqual("<Content />", xmlDocument.InnerXml);
        }

        [TestMethod()]
        public void DeSerializeFlatStringArray()
        {
            static Stream stream(string x) => new MemoryStream(Encoding.Unicode.GetBytes(x));

            const string testInput = "<Content>" +
                "<Item>a</Item>" +
                "<Item>b</Item>" +
                "<Item>c</Item>" +
                "</Content>";

            // load from XML
            XmlDocument xmlDocument = new();
            xmlDocument.Load(stream(testInput));

            XmlDocument interpreterDocument = new();
            interpreterDocument.Load(stream("<Converts><Converts>" +
                "<Convert Path=\"//Item\" Type=\"String\" Encoding=\"UTF-8\"/>" +
                "</Converts></Converts>"));
            XmlDocument xmlWithBytes = new FileDBReader.XmlExporter(xmlDocument, new(interpreterDocument)).Run();
            BBDocument doc = new XmlToBBDocumentConverter().ToBBDocument(xmlWithBytes);

            Assert.AreEqual(3, doc.Roots.Count);
            Assert.IsTrue(doc.Roots[0] is Attrib);
            Assert.AreEqual("Item", doc.Roots[0].Name);

            Assert.IsTrue(doc.TagSection.Attribs.Values.Contains("Item"));  // make sure "Item" is only added as Attrib
            Assert.IsTrue(!doc.TagSection.Tags.Values.Contains("Item"));

            // deserialize & serialize
            BBDocumentDeserializer<FlatStringArrayContainer> deserializer = new(new() { Version = BBDocumentVersion.V1 });
            var obj = deserializer.GetObjectStructureFromFileDBDocument(doc);

            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.Item);
            Assert.AreEqual(3, obj.Item!.Count);
            Assert.AreEqual("a", obj.Item[0]);
            Assert.AreEqual("b", obj.Item[1]);
            Assert.AreEqual("c", obj.Item[2]);

            BBDocumentSerializer serializer = new(new() { Version = BBDocumentVersion.V1 });
            doc = serializer.WriteObjectStructureToFileDBDocument(obj);

            Assert.IsTrue(doc.TagSection.Attribs.Values.Contains("Item"));  // make sure "Item" is only added as Attrib
            Assert.IsTrue(!doc.TagSection.Tags.Values.Contains("Item"));

            // convert back to xml
            xmlWithBytes = new FileDbXmlConverter().ToXml(doc);
            xmlDocument = new FileDBReader.XmlInterpreter(xmlWithBytes, new(interpreterDocument)).Run();
            Assert.AreEqual(testInput, xmlDocument.InnerXml);
        }

        private class PrimitiveListArrayContainer
        {
            public byte[]? Single { get; set; }
            public List<byte[]>? NonFlat { get; set; } = new();
            [FlatArray]
            public List<byte[]>? Flat { get; set; } = new();
        }

        [TestMethod()]
        public void DeSerializePrimitiveArray()
        {
            static Stream stream(string x) => new MemoryStream(Encoding.Unicode.GetBytes(x));

            const string testInput = "<Content>" +
                "<Single>00013434</Single>" +
                "<NonFlat><None>00013535</None></NonFlat>" +
                "<Flat>00013636</Flat>" +
                "<Flat>00013737</Flat>" +
                "</Content>";

            // load from XML
            XmlDocument xmlDocument = new();
            xmlDocument.Load(stream(testInput));
            BBDocument doc = new XmlToBBDocumentConverter().ToBBDocument(xmlDocument);

            // serialize & deserialize
            BBDocumentDeserializer<PrimitiveListArrayContainer> deserializer = new(new() { Version = BBDocumentVersion.V1 });
            var obj = deserializer.GetObjectStructureFromFileDBDocument(doc);

            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.Single);
            Assert.AreEqual("00-01-34-34", BitConverter.ToString(obj.Single!));
            Assert.AreEqual(1, obj.NonFlat?.Count);
            Assert.AreEqual("00-01-35-35", BitConverter.ToString(obj.NonFlat![0]));
            Assert.AreEqual(2, obj.Flat?.Count);
            Assert.AreEqual("00-01-36-36", BitConverter.ToString(obj.Flat![0]));
            Assert.AreEqual("00-01-37-37", BitConverter.ToString(obj.Flat[1]));

            BBDocumentSerializer serializer = new(new() { Version = BBDocumentVersion.V1 });
            doc = serializer.WriteObjectStructureToFileDBDocument(obj);

            // convert back to xml
            xmlDocument = new FileDbXmlConverter().ToXml(doc);
            Assert.AreEqual(testInput, xmlDocument.InnerXml);
        }

        private class EmptyAttribTestContainer
        {
            public byte[]? Single { get; set; }
            public byte[]? MissingSingle { get; set; }
            public string? EmptyString { get; set; }
            public List<byte[]>? NonFlat { get; set; }
            public object? ThisIsSelfclosing { get; set; }
            public int? MissingNullableInt { get; set; }
        }

        [TestMethod]
        public void DeSerializeEmptyPrimitiveArray()
        {
            static Stream stream(string x) => new MemoryStream(Encoding.Unicode.GetBytes(x));

            const string testInput = "<Content>" +
                "<Single></Single>" +
                "<EmptyString></EmptyString>" +
                "<NonFlat><None></None></NonFlat>" +
                "<ThisIsSelfclosing />" +
                "</Content>";

            // load from XML
            XmlDocument xmlDocument = new();
            xmlDocument.Load(stream(testInput));
            BBDocument doc = new XmlToBBDocumentConverter().ToBBDocument(xmlDocument);

            // serialize & deserialize
            BBDocumentDeserializer<EmptyAttribTestContainer> deserializer = new(new() { Version = BBDocumentVersion.V1 });
            var obj = deserializer.GetObjectStructureFromFileDBDocument(doc);

            Assert.IsNotNull(obj);

            Assert.IsNotNull(obj.Single);
            Assert.AreEqual(0, obj.Single!.Length);

            Assert.IsNotNull(obj.EmptyString);
            Assert.AreEqual(0, obj.EmptyString!.Length);

            Assert.IsNotNull(obj.NonFlat);
            Assert.AreEqual(1, obj.NonFlat?.Count);
            Assert.AreEqual(0, obj.NonFlat![0].Length);

            Assert.IsNotNull(obj.ThisIsSelfclosing);

            Assert.IsNull(obj.MissingNullableInt);

            BBDocumentSerializer serializer = new(new() { Version = BBDocumentVersion.V1 });
            doc = serializer.WriteObjectStructureToFileDBDocument(obj);

            // convert back to xml
            xmlDocument = new FileDbXmlConverter().ToXml(doc);
            Assert.AreEqual(testInput, xmlDocument.InnerXml);
        }
    }
}
