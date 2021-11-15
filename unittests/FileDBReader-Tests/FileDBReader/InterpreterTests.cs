using FileDBReader.src;
using FileDBReader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace FileDBReader_Tests
{
    [TestClass]
    public class InterpreterTests
    {
        [TestMethod]
        public void DoesNotCrashOnRetardedInterpreter()
        {
            //this just has to run. The interpreter is as broken as it can get, we just don't want to crash the application :D
            String INPUT_FILENAME = "interpreter_broken.xml";
            var path = Path.Combine(Folders.UNITTEST_INTERPRETER_DIR, INPUT_FILENAME);
            Interpreter Interpr = new Interpreter(Interpreter.ToInterpreterDoc(path));
        }

        [TestMethod]
        public void DoesNotCrashOnFaultyXmlInterpreter()
        {
            //this just has to run. The interpreter is as broken as it can get, we just want to see if it crashes the application :D
            String INPUT_FILENAME = "interpreter_brokenxml.xml";
            var path = Path.Combine(Folders.UNITTEST_INTERPRETER_DIR, INPUT_FILENAME);
            Interpreter Interpr = new Interpreter(Interpreter.ToInterpreterDoc(path));
        }

        [TestMethod]
        public void GetsCorrectCombinedXpath()
        {
            Interpreter i = new Interpreter();
            i.Conversions.Add(("//This/Is/A/Default/Path", new Conversion() { Type = typeof(int), Structure = ContentStructure.Default}));
            i.Conversions.Add(("//This/Is/Another/Path", new Conversion() { Type = typeof(String), Structure = ContentStructure.Default }));
            i.InternalCompressions.Add(new InternalCompression { Path = "./InternalCompression" });
            String ExpectedInverseXpath = "//This/Is/A/Default/Path | //This/Is/Another/Path | ./InternalCompression";

            String Combined = i.GetCombinedXPath();

            Assert.AreEqual(ExpectedInverseXpath, Combined);
        }

        [TestMethod]
        public void DetectsFaultyXpath()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Convert Path = \" F a u lt y/ Xpath[ this =]\"/>");
            var node = doc.FirstChild; 

            Interpreter i = new Interpreter();
            Assert.ThrowsException<XPathException>( () => i.GetPath(node));
        }

        [TestMethod]
        public void InterpreterSerializing_DefaultType()
        {
            String INPUT_FILENAME = "Interpreter_demo.xml";
            var path = Path.Combine(Folders.UNITTEST_INTERPRETER_DIR, INPUT_FILENAME);
            Interpreter InterpreterSerialized = new Interpreter(Interpreter.ToInterpreterDoc(path));
            Interpreter InterpreterManual = BuildTestInterpreter();

            //assert default type
            Assert.AreEqual(InterpreterManual.DefaultType.Encoding, InterpreterSerialized.DefaultType.Encoding);
            Assert.AreEqual(InterpreterManual.DefaultType.Structure, InterpreterSerialized.DefaultType.Structure);
            Assert.AreEqual(InterpreterManual.DefaultType.Type, InterpreterSerialized.DefaultType.Type);

            //check enum
            foreach (KeyValuePair<String, String> k in InterpreterManual.DefaultType.Enum)
            {
                if (InterpreterSerialized.DefaultType.Enum.ContainsKey(k.Key))
                    Assert.AreEqual(k.Value, InterpreterSerialized.DefaultType.Enum[k.Key]);
                else Assert.Fail();
            }
        }

        [TestMethod]
        public void InterpreterSerializing_Conversion() 
        {
            String INPUT_FILENAME = "Interpreter_demo.xml";
            var path = Path.Combine(Folders.UNITTEST_INTERPRETER_DIR, INPUT_FILENAME);
            Interpreter InterpreterSerialized = new Interpreter(Interpreter.ToInterpreterDoc(path));
            Interpreter InterpreterManual = BuildTestInterpreter(); 

            //Assert all conversions in this interpreter!!!
            var CompressionsManual = InterpreterManual.Conversions;
            var CompressionsSerialized = InterpreterSerialized.Conversions;

            if (CompressionsManual.Count == CompressionsSerialized.Count)
            {
                for (int i = 0; i < CompressionsManual.Count; i++)
                { 
                    Conversion e_manual = CompressionsManual[i].Item2;
                    Conversion e_serial = CompressionsSerialized[i].Item2;

                    Assert.AreEqual(e_manual.Type, e_serial.Type);
                    Assert.AreEqual(e_manual.Structure, e_serial.Structure);
                    Assert.AreEqual(e_manual.Encoding, e_serial.Encoding);

                    foreach (KeyValuePair<String, String> f in e_manual.Enum)
                    {
                        if (e_serial.Enum.ContainsKey(f.Key))
                            Assert.AreEqual(f.Value, e_serial.Enum[f.Key]);
                        else Assert.Fail();
                    }
                }
            }
            else Assert.Fail();
        }

        [TestMethod]
        public void InterpreterSerializing_InternalCompression()
        {
            String INPUT_FILENAME = "Interpreter_demo.xml";
            var path = Path.Combine(Folders.UNITTEST_INTERPRETER_DIR, INPUT_FILENAME);
            Interpreter InterpreterSerialized = new Interpreter(Interpreter.ToInterpreterDoc(path));
            Interpreter InterpreterManual = BuildTestInterpreter();

            //Assert internal Compression
            var InternalCompressionsManual = InterpreterManual.InternalCompressions;
            var InternalCompressionsSerialized = InterpreterSerialized.InternalCompressions;
            if (InternalCompressionsManual.Count == InternalCompressionsSerialized.Count)
            {
                for (int i = 0; i < InternalCompressionsManual.Count; i++)
                {
                    Assert.AreEqual(InternalCompressionsManual[i].Path, InternalCompressionsSerialized[i].Path);
                    Assert.AreEqual(InternalCompressionsManual[i].Path, InternalCompressionsSerialized[i].Path);
                }
            }
            else Assert.Fail();
        }

        [TestMethod]
        ///Tests if results match after converting a value through a RunTimeEnum: x -> y -> x must be true. 
        public void EnumConversion_Equivalence()
        { 
            //Setup
            RuntimeEnum TestEnum = new RuntimeEnum();
            TestEnum.AddValue("0", "ValueZero");
            TestEnum.AddValue("1", "ValueOne");
            TestEnum.AddValue("2", "ValueTwo");
            String TestString = "1";

            //Execute
            String TestStringEnumValue_ToValue = TestEnum.GetValue(TestString);
            String TestStringEnumValue_ToValue_ToKey = TestEnum.GetKey(TestStringEnumValue_ToValue);

            //Assert
            Assert.IsTrue(TestString.Equals(TestStringEnumValue_ToValue_ToKey));
        }

        [TestMethod]
        public void SafeAdd_Detects_Duplicate()
        {
            Dictionary<String, String> dict = new Dictionary<String, String>();
            //Setup
            RuntimeEnum TestEnum = new RuntimeEnum();
            dict.Add("0", "ValueZero");
            dict.Add("1", "ValueOne");
            dict.Add("2", "ValueTwo");

            Assert.ThrowsException<ArgumentException>(() => dict.SafeAdd("3","ValueTwo"));
        }

        #region Helpful
        private Interpreter BuildTestInterpreter()
        {
            //build the same interpreter from code
            Interpreter InterpreterManual = new Interpreter();

            List<InternalCompression> list = new List<InternalCompression>();
            list.Add(new InternalCompression() { CompressionVersion = 2, Path = "//AreaManagerData/None/Data" });
            InterpreterManual.InternalCompressions = list;

            var Compressions = new List <(string, Conversion)> ();

            //SetupEnum for later use
            RuntimeEnum TestEnum = new RuntimeEnum();
            TestEnum.AddValue("0", "Small");
            TestEnum.AddValue("1", "Medium");
            TestEnum.AddValue("2", "Large");

            Compressions.Add( ("//VegetationPropSetName", new Conversion() { Type = typeof(String), Encoding = Encoding.GetEncoding("UTF-8") } ));
            Compressions.Add(("//GlobalAmbientName", new Conversion() { Type = typeof(String) }));
            Compressions.Add(("//HeightMap/HeightMap", new Conversion() { Type = typeof(System.UInt16), Structure = ContentStructure.List }));
            Compressions.Add(("//MapTemplate/TemplateElement/Element/Size", new Conversion() { Type = typeof(System.Int16), Enum = TestEnum }));
            InterpreterManual.Conversions = Compressions;

            InterpreterManual.DefaultType = new Conversion() { Type = typeof(System.Int32) };
            return InterpreterManual;
        }
        #endregion
    }
}
