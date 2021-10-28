using FileDBReader.src;
using FileDBReader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace FileDBReader_Tests
{
    [TestClass]
    public class InterpreterTests
    {

        //FileDB
        static FileReader reader = new FileReader();
        static XmlExporter exporter = new XmlExporter();
        static FileWriter writer = new FileWriter();
        static XmlInterpreter interpreter = new XmlInterpreter();
        static ZlibFunctions zlib = new ZlibFunctions();

        //Fc Files
        static FcFileHelper FcFileHelper = new FcFileHelper();

        [TestMethod]
        public void TestMethod1()
        {

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
                foreach (KeyValuePair<String, Conversion> k in CompressionsManual)
                {
                    if (CompressionsSerialized.ContainsKey(k.Key))
                    {
                        //right here we can already assume that the paths match. 
                        Conversion ConversionManual = k.Value;
                        Conversion ConversionSerialized = InterpreterSerialized.Conversions[k.Key];
                        Assert.AreEqual(ConversionManual.Type, ConversionSerialized.Type);
                        Assert.AreEqual(ConversionManual.Structure, ConversionSerialized.Structure);
                        Assert.AreEqual(ConversionManual.Encoding, ConversionSerialized.Encoding);

                        //Enum
                        foreach (KeyValuePair<String, String> f in ConversionManual.Enum)
                        {
                            if (ConversionSerialized.Enum.ContainsKey(f.Key))
                                Assert.AreEqual(f.Value, ConversionSerialized.Enum[f.Key]);
                            else Assert.Fail();
                        }
                    }
                    else Assert.Fail();
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


        #region Helpful
        private Interpreter BuildTestInterpreter()
        {
            //build the same interpreter from code
            Interpreter InterpreterManual = new Interpreter();

            List<InternalCompression> list = new List<InternalCompression>();
            list.Add(new InternalCompression() { CompressionVersion = 2, Path = "//AreaManagerData/None/Data" });
            InterpreterManual.InternalCompressions = list;

            var Compressions = new Dictionary<string, Conversion>();

            //SetupEnum for later use
            RuntimeEnum TestEnum = new RuntimeEnum();
            TestEnum.AddValue("0", "Small");
            TestEnum.AddValue("1", "Medium");
            TestEnum.AddValue("2", "Large");

            Compressions.Add("//VegetationPropSetName", new Conversion() { Type = typeof(String), Encoding = Encoding.GetEncoding("UTF-8") });
            Compressions.Add("//GlobalAmbientName", new Conversion() { Type = typeof(String) });
            Compressions.Add("//HeightMap/HeightMap", new Conversion() { Type = typeof(System.UInt16), Structure = ContentStructure.List });
            Compressions.Add("//MapTemplate/TemplateElement/Element/Size", new Conversion() { Type = typeof(System.Int16), Enum = TestEnum });
            InterpreterManual.Conversions = Compressions;

            InterpreterManual.DefaultType = new Conversion() { Type = typeof(System.Int32) };
            return InterpreterManual;
        }

        /*
        private void GenericFileTest(String DIRECTORY_NAME, String INTERPREFER_FILE_NAME, String TESTFILE_NAME, int FileVersion)
        {
            String INTERPRETER_FILE = Path.Combine(Folders.FILEFORMAT_DIRECTORY_NAME, INTERPREFER_FILE_NAME);
            String TESTFILE = Path.Combine(Folders.TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME);

            String DECOMPRESSED_TESTFILE = Path.Combine(Folders.TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_decompressed.xml");
            String INTERPRETED_TESTFILE = Path.Combine(Folders.TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_interpreted.xml");
            String TOHEX_TESTFILE = Path.Combine(Folders.TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_reinterpreted.xml");
            String EXPORTED_TESTFILE = Path.Combine(Folders.TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_recompressed" + Path.GetExtension(TESTFILE_NAME));

            //decompress
            var decompressed = reader.ReadFile(TESTFILE);
            decompressed.Save(DECOMPRESSED_TESTFILE);

            var interpreted = interpreter.Interpret(decompressed, new Interpreter(Interpreter.ToInterpreterDoc(INTERPRETER_FILE)));
            interpreted.Save(INTERPRETED_TESTFILE);

            //to hex 
            var Hexed = exporter.Export(interpreted, new Interpreter(Interpreter.ToInterpreterDoc(INTERPRETER_FILE)));
            Hexed.Save(TOHEX_TESTFILE);

            //back to gamedata 
            writer.Export(Hexed, EXPORTED_TESTFILE, FileVersion);

            var OriginalInfo = new FileInfo(TESTFILE);
            var DecompressedInfo = new FileInfo(DECOMPRESSED_TESTFILE);
            var InterpretedInfo = new FileInfo(INTERPRETED_TESTFILE);
            var RehexedInfo = new FileInfo(TOHEX_TESTFILE);
            var ReexportedInfo = new FileInfo(EXPORTED_TESTFILE);
        }
        */
        #endregion
    }
}
