using AnnoMods.BBDom;
using AnnoMods.BBDom.XML;
using AnnoMods.ObjectSerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Xml;

namespace FileDBReader_Tests.FileDBSerializer
{
    internal class PolymorphSerializerTests
    {
        private class PolymorphOnPathContainer
        {
            public class BaseItem
            {
            }

            public class ValueItem : BaseItem
            {
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles")]
                public Value? value { get; set; }
            }

            public class Value
            {
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles")]
                public int? id { get; set; }
            }

            public class ComplexItem : BaseItem
            {
                // TODO the current serializer can't do non-flat within non-flat arrays
                [FlatArray]
                public Complex[]? None { get; set; }
            }

            public class Complex
            {
                public int? ObjectId { get; set; }
            }

            [Polymorph(typeof(ValueItem), "value/id")]
            [Polymorph(typeof(ComplexItem), "None/ObjectId")]
            public BaseItem[]? List { get; set; }
        }

        [TestMethod()]
        public void DeSerializePolymorphOnPathArray()
        {
            static Stream stream(string x) => new MemoryStream(Encoding.Unicode.GetBytes(x));

            const string testInput = "<Content><List>" +
                "<None>" +
                    "<value>" +
                        "<id>03000000</id>" +
                    "</value>" +
                "</None>" +
                "<None>" +
                    "<None>" +
                        "<ObjectId>01000000</ObjectId>" +
                    "</None>" +
                    "<None>" +
                        "<ObjectId>02000000</ObjectId>" +
                    "</None>" +
                "</None>" +
              "</List></Content>";

            // load from XML
            XmlDocument xmlDocument = new();
            xmlDocument.Load(stream(testInput));
            BBDocument doc = xmlDocument.ToBBDocument();

            // serialize & deserialize
            BBDocumentDeserializer<PolymorphOnPathContainer> deserializer = new(new() { Version = BBDocumentVersion.V1 });
            var obj = deserializer.GetObjectStructureFromBBDocument(doc);

            Assert.IsNotNull(obj);
            var first = obj.List?[0] as PolymorphOnPathContainer.ValueItem;
            Assert.AreEqual(3, first?.value?.id);
            var second = obj.List?[1] as PolymorphOnPathContainer.ComplexItem;
            Assert.AreEqual(1, second?.None?[0]?.ObjectId);
            var third = obj.List?[1] as PolymorphOnPathContainer.ComplexItem;
            Assert.AreEqual(2, third?.None?[1]?.ObjectId);

            BBDocumentSerializer serializer = new(new() { Version = BBDocumentVersion.V1 });
            doc = serializer.WriteObjectStructureToBBDocument(obj);

            // convert back to xml
            xmlDocument = doc.ToXmlDocument();
            Assert.AreEqual(testInput, xmlDocument.InnerXml);
        }

        private class PolymorphOnValueContainer
        {
            public class PolymorphTemplateElement
            {
                public int? ElementType { get; set; }

                [Polymorph(typeof(ElementType0), "ElementType", HexValue = "00000000", Node = PolymorphRootNode.Parent)]
                [Polymorph(typeof(ElementType1), "ElementType", HexValue = "00000001", Node = PolymorphRootNode.Parent)]
                public Element? Element { get; set; }
            }

            public class Element
            {
                public int[]? Position { get; set; }
            }

            public class ElementType0 : Element
            {
                public byte? Rotation90 { get; set; }
            }

            public class ElementType1 : Element
            {
                public short? Size { get; set; }
            }

            [FlatArray]
            public PolymorphTemplateElement[]? TemplateElement { get; set; }
        }

        [TestMethod()]
        public void DeSerializePolymorphOnValueArray()
        {
            static Stream stream(string x) => new MemoryStream(Encoding.Unicode.GetBytes(x));

            const string testInput = "<Content>" +
                "<TemplateElement>" +
                    "<ElementType>00000000</ElementType>" +
                    "<Element>" +
                        "<Position>0100000002000000</Position>" +
                        "<Rotation90>01</Rotation90>" +
                    "</Element>" +
                "</TemplateElement>" +
                "<TemplateElement>" +
                    "<ElementType>00000001</ElementType>" +
                    "<Element>" +
                        "<Position>0200000003000000</Position>" +
                        "<Size>0500</Size>" +
                    "</Element>" +
                "</TemplateElement>" +
                "<TemplateElement>" +
                    "<ElementType>00000002</ElementType>" +
                    "<Element>" +
                        "<Position>0300000004000000</Position>" +
                    "</Element>" +
                "</TemplateElement>" +
              "</Content>";

            // load from XML
            XmlDocument xmlDocument = new();
            xmlDocument.Load(stream(testInput));
            BBDocument doc = xmlDocument.ToBBDocument();

            // serialize & deserialize
            BBDocumentDeserializer<PolymorphOnValueContainer> deserializer = new(new() { Version = BBDocumentVersion.V1 });
            var obj = deserializer.GetObjectStructureFromBBDocument(doc);

            Assert.IsNotNull(obj);
            Assert.AreEqual(3, obj.TemplateElement?.Length);
            var first = obj.TemplateElement?[0].Element as PolymorphOnValueContainer.ElementType0;
            Assert.AreEqual(1, first?.Position?[0]);
            Assert.AreEqual((byte)1, first?.Rotation90);
            var second = obj.TemplateElement?[1].Element as PolymorphOnValueContainer.ElementType1;
            Assert.AreEqual(2, second?.Position?[0]);
            Assert.AreEqual((short)5, second?.Size);
            var third = obj.TemplateElement?[2].Element;
            Assert.AreEqual(3, third?.Position?[0]);

            BBDocumentSerializer serializer = new(new() { Version = BBDocumentVersion.V1 });
            doc = serializer.WriteObjectStructureToBBDocument(obj);

            // convert back to xml
            xmlDocument = doc.ToXmlDocument();
            Assert.AreEqual(testInput, xmlDocument.InnerXml);
        }
    }
}
