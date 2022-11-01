using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FileDBReader.src.XmlRepresentation;
using FileDBReader_Tests;
using FileDBSerializing.ObjectSerializer;
using FileDBSerializing.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using FileDBReader_Tests.TestSerializationData.PropertyOrder;

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
            FileDBSerializer<RootObject> objectserializer = new FileDBSerializer<RootObject>(FileDBDocumentVersion.Version1);
            
            Stream result = new MemoryStream();
            objectserializer.Serialize(result, obj);

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, result));
        }

        [TestMethod]
        public void SerializeTest_V2()
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version2.filedb");

            var obj = TestDataSources.GetTestAsset();
            FileDBSerializer<RootObject> objectserializer = new FileDBSerializer<RootObject>(FileDBDocumentVersion.Version2);
            MemoryStream result = new MemoryStream();
            objectserializer.Serialize(result, obj);

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, result));
        }

        [TestMethod]
        public void DeserializeTest_V1()
        {
            var x = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version1.filedb");

            DocumentParser parser = new DocumentParser(FileDBDocumentVersion.Version1);
            IFileDBDocument doc = parser.LoadFileDBDocument(x);

            FileDBDocumentDeserializer<RootObject> objectdeserializer = new FileDBDocumentDeserializer<RootObject>(new() { Version = FileDBDocumentVersion.Version1});

            var DeserializedDocument = objectdeserializer.GetObjectStructureFromFileDBDocument(doc);

            DeserializedDocument.Should().BeEquivalentTo(TestDataSources.GetTestAsset());
        }

        [TestMethod]
        public void DeserializeTest_V2()
        {
            var x = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version2.filedb");

            DocumentParser parser = new DocumentParser(FileDBDocumentVersion.Version2);
            IFileDBDocument doc = parser.LoadFileDBDocument(x);

            FileDBDocumentDeserializer<RootObject> objectdeserializer = new FileDBDocumentDeserializer<RootObject>(new() { Version = FileDBDocumentVersion.Version1 });

            var DeserializedDocument = objectdeserializer.GetObjectStructureFromFileDBDocument(doc);

            DeserializedDocument.Should().BeEquivalentTo(TestDataSources.GetTestAsset());
        }

        [TestMethod]
        public void StaticConvertTest_Serialize()
        {
            var expected = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version2.filedb");

            var obj = TestDataSources.GetTestAsset();

            Stream Result = FileDBConvert.SerializeObject(obj, new() { Version = FileDBDocumentVersion.Version2 });

            Assert.IsTrue(FileConversionTests.StreamsAreEqual(expected, Result));
        }

        [TestMethod]
        public void StaticConvertTest_Deserialize()
        {
            var source = File.OpenRead("FileDBSerializer/Testfiles/objectserializing/version2.filedb");
            RootObject? result = FileDBConvert.DeserializeObject<RootObject>(source, new() { Version = FileDBDocumentVersion.Version2 });

            result.Should().BeEquivalentTo(TestDataSources.GetTestAsset());
        }


        [TestMethod()]
        public void SkipSimpleNullValues()
        {
            // test default setting
            FileDBSerializerOptions options = new() { Version = FileDBDocumentVersion.Version1 };
            Assert.IsTrue(options.SkipSimpleNullValues); 
            
            // all null
            var obj = new RootObject();
            FileDBDocumentSerializer serializer = new(new() { Version = FileDBDocumentVersion.Version1 });
            IFileDBDocument doc = serializer.WriteObjectStructureToFileDBDocument(obj);
            XmlDocument xmlDocument = new FileDbXmlConverter().ToXml(doc);
            Assert.AreEqual("<Content />", xmlDocument.InnerXml);

            // all null, SkipReferenceArrayNullValues = false
            serializer = new(new() { Version = FileDBDocumentVersion.Version1, SkipReferenceArrayNullValues = false });
            doc = serializer.WriteObjectStructureToFileDBDocument(obj);
            xmlDocument = new FileDbXmlConverter().ToXml(doc);
            Assert.AreEqual(
                "<Content>" +
                "<RefArray />" +
                "</Content>", xmlDocument.InnerXml);

            // all null, SkipSimpleNullValues = false
            serializer = new(new() { Version = FileDBDocumentVersion.Version1, SkipSimpleNullValues = false });
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
            serializer = new(new() { Version = FileDBDocumentVersion.Version1, SkipSimpleNullValues = false, SkipListNullValues = false });
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
            serializer = new(new() { Version = FileDBDocumentVersion.Version1, SkipSimpleNullValues = false, SkipListNullValues = false, SkipReferenceArrayNullValues = false });
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

            XmlFileDbConverter xmlFileDbConverter = new XmlFileDbConverter(FileDBDocumentVersion.Version1);

            IFileDBDocument doc = xmlFileDbConverter.ToFileDb(xmlDoc);

            FileDBDocumentDeserializer<RootObject> objectdeserializer = new FileDBDocumentDeserializer<RootObject>(new() { Version = FileDBDocumentVersion.Version1 });

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

            XmlFileDbConverter xmlFileDbConverter = new XmlFileDbConverter(FileDBDocumentVersion.Version1);

            IFileDBDocument fromXML = xmlFileDbConverter.ToFileDb(xmlDoc);
            FileDBDocumentDeserializer<RootObject> objectdeserializer = new FileDBDocumentDeserializer<RootObject>(new() { Version = FileDBDocumentVersion.Version1 });
            RootObject? DeserializedDocument = objectdeserializer.GetObjectStructureFromFileDBDocument(fromXML);

            FileDBDocumentSerializer serializer = new(new() { Version = FileDBDocumentVersion.Version1 });
            IFileDBDocument toXml = serializer.WriteObjectStructureToFileDBDocument(DeserializedDocument);
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

            FileDBDocumentSerializer serializer = new(new() { Version = FileDBDocumentVersion.Version1 });
            IFileDBDocument doc = serializer.WriteObjectStructureToFileDBDocument(obj);
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

            FileDBDocumentSerializer serializer = new(new() { Version = FileDBDocumentVersion.Version1 });
            IFileDBDocument doc = serializer.WriteObjectStructureToFileDBDocument(obj);
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

            FileDBDocumentSerializer serializer = new(new() { Version = FileDBDocumentVersion.Version1 });
            IFileDBDocument doc = serializer.WriteObjectStructureToFileDBDocument(obj);
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

            FileDBDocumentSerializer serializer = new(new() { Version = FileDBDocumentVersion.Version1 });
            IFileDBDocument doc = serializer.WriteObjectStructureToFileDBDocument(obj);
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
            IFileDBDocument doc = new XmlFileDbConverter(FileDBDocumentVersion.Version1).ToFileDb(xmlDocument);

            // serialize & deserialize
            FileDBDocumentDeserializer<FlatStringArrayContainer> deserializer = new(new() { Version = FileDBDocumentVersion.Version1 });
            var obj = deserializer.GetObjectStructureFromFileDBDocument(doc);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.Item!.Count == 0);

            FileDBDocumentSerializer serializer = new(new() { Version = FileDBDocumentVersion.Version1 });
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
            XmlDocument xmlWithBytes = new FileDBReader.XmlExporter().Export(xmlDocument, new(interpreterDocument));
            IFileDBDocument doc = new XmlFileDbConverter(FileDBDocumentVersion.Version1).ToFileDb(xmlWithBytes);

            Assert.AreEqual(3, doc.Roots.Count);
            Assert.IsTrue(doc.Roots[0] is Attrib);
            Assert.AreEqual("Item", doc.Roots[0].Name);

            Assert.IsTrue(doc.Tags.Attribs.ContainsValue("Item"));  // make sure "Item" is only added as Attrib
            Assert.IsTrue(!doc.Tags.Tags.ContainsValue("Item"));

            // deserialize & serialize
            FileDBDocumentDeserializer<FlatStringArrayContainer> deserializer = new(new() { Version = FileDBDocumentVersion.Version1 });
            var obj = deserializer.GetObjectStructureFromFileDBDocument(doc);

            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.Item);
            Assert.AreEqual(3, obj.Item!.Count);
            Assert.AreEqual("a", obj.Item[0]);
            Assert.AreEqual("b", obj.Item[1]);
            Assert.AreEqual("c", obj.Item[2]);

            FileDBDocumentSerializer serializer = new(new() { Version = FileDBDocumentVersion.Version1 });
            doc = serializer.WriteObjectStructureToFileDBDocument(obj);

            Assert.IsTrue(doc.Tags.Attribs.ContainsValue("Item"));  // make sure "Item" is only added as Attrib
            Assert.IsTrue(!doc.Tags.Tags.ContainsValue("Item"));

            // convert back to xml
            xmlWithBytes = new FileDbXmlConverter().ToXml(doc);
            xmlDocument = new FileDBReader.XmlInterpreter().Interpret(xmlWithBytes, new(interpreterDocument));
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
            IFileDBDocument doc = new XmlFileDbConverter(FileDBDocumentVersion.Version1).ToFileDb(xmlDocument);

            // serialize & deserialize
            FileDBDocumentDeserializer<PrimitiveListArrayContainer> deserializer = new(new() { Version = FileDBDocumentVersion.Version1 });
            var obj = deserializer.GetObjectStructureFromFileDBDocument(doc);

            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.Single);
            Assert.AreEqual("00-01-34-34", BitConverter.ToString(obj.Single!));
            Assert.AreEqual(1, obj.NonFlat?.Count);
            Assert.AreEqual("00-01-35-35", BitConverter.ToString(obj.NonFlat![0]));
            Assert.AreEqual(2, obj.Flat?.Count);
            Assert.AreEqual("00-01-36-36", BitConverter.ToString(obj.Flat![0]));
            Assert.AreEqual("00-01-37-37", BitConverter.ToString(obj.Flat[1]));

            FileDBDocumentSerializer serializer = new(new() { Version = FileDBDocumentVersion.Version1 });
            doc = serializer.WriteObjectStructureToFileDBDocument(obj);

            // convert back to xml
            xmlDocument = new FileDbXmlConverter().ToXml(doc);
            Assert.AreEqual(testInput, xmlDocument.InnerXml);
        }
    }
}
