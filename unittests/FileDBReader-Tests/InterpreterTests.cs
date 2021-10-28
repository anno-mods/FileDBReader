using FileDBReader.src;
using FileDBReader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace FileDBReader_Tests
{
    [TestClass]
    public class InterpreterTests
    {
        static readonly String TEST_DIRECTORY_NAME = "tests";
        static readonly String FILEFORMAT_DIRECTORY_NAME = "FileFormats";

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
        ///Tests if results match after converting a value through a RunTimeEnum: x -> y -> x must be true. 
        public void EnumConversionTest()
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

        public static void GenericFileTest(String DIRECTORY_NAME, String INTERPREFER_FILE_NAME, String TESTFILE_NAME, int FileVersion)
        {
            String INTERPRETER_FILE = Path.Combine(FILEFORMAT_DIRECTORY_NAME, INTERPREFER_FILE_NAME);
            String TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME);

            String DECOMPRESSED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_decompressed.xml");
            String INTERPRETED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_interpreted.xml");
            String TOHEX_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_reinterpreted.xml");
            String EXPORTED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_recompressed" + Path.GetExtension(TESTFILE_NAME));

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

            Console.WriteLine("File Test: {0}", TESTFILE_NAME);
            Console.WriteLine("Used FileDBCompression Version for re-export: {0}", FileVersion);
            Console.WriteLine("FILEDB FILES FILESIZE\nOriginal: {0}, Converted: {1}. Filesize Equality:{2}", OriginalInfo.Length, ReexportedInfo.Length, OriginalInfo.Length == ReexportedInfo.Length);
            //This check will probably give a false if there is internal compression!
            Console.WriteLine("XML FILES FILESIZE\n Decompressed: {0}, Recompressed: {1}. Filesize Equality: {2}", DecompressedInfo.Length, RehexedInfo.Length, DecompressedInfo.Length == RehexedInfo.Length);

            Console.WriteLine("File Test Done");
            Console.WriteLine("--------------------------------------------------");
        }
    }
}
