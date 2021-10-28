using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileDBReader;
using System.Xml;
using System.IO;
using FileDBReader.src;

namespace FileDBReader_Tests
{
    [TestClass]
    public class FileConversionTests
    {
        //FileDB
        static FileReader reader = new FileReader();
        static XmlExporter exporter = new XmlExporter();
        static FileWriter writer = new FileWriter();
        static XmlInterpreter interpreter = new XmlInterpreter();

        //Fc Files
        static FcFileHelper FcFileHelper = new FcFileHelper();

        #region VERSION_TEST

        [TestMethod, TestCategory("version")]
        public void Detects_Version1()
        {
            String UNITTEST_EXPECTED_SUBDIR = "version";
            String INPUT_FILENAME = "version1.tmc";
            var STRING_PATH = Path.Combine(Folders.UNITTEST_FILE_DIR, Folders.UNITTEST_FILE_TESTFILES_DIR, UNITTEST_EXPECTED_SUBDIR, INPUT_FILENAME);
            Assert.IsTrue(reader.CheckFileVersion(STRING_PATH) == 1);
        }

        [TestMethod, TestCategory("version")]
        public void Detects_Version2()
        {
            String UNITTEST_EXPECTED_SUBDIR = "version";
            String INPUT_FILENAME = "version2.bin";
            var STRING_PATH = Path.Combine(Folders.UNITTEST_FILE_DIR, Folders.UNITTEST_FILE_TESTFILES_DIR, UNITTEST_EXPECTED_SUBDIR, INPUT_FILENAME);
            Assert.IsTrue(reader.CheckFileVersion(STRING_PATH) == 2);
        }

        #endregion


        #region A7TINFOTEST

        [TestMethod, TestCategory("a7tinfo")]
        public void a7tinfo_Decompress()
        {
            String UNITTEST_EXPECTED_SUBDIR = "a7tinfo";
            String COMPARE_DECOMPRESSED_FILE = "decompressed.xml";
            String INPUT_FILENAME = "testfile.a7tinfo";

            var COMPARE_PATH = Path.Combine(Folders.UNITTEST_FILE_DIR, Folders.UNITTEST_FILE_EXPECTED_DIR, UNITTEST_EXPECTED_SUBDIR, COMPARE_DECOMPRESSED_FILE);
            var INPUT_PATH = Path.Combine(Folders.UNITTEST_FILE_DIR, Folders.UNITTEST_FILE_TESTFILES_DIR, UNITTEST_EXPECTED_SUBDIR, INPUT_FILENAME);

            bool b = Test_Decompress(INPUT_PATH, COMPARE_PATH);
            Assert.IsTrue(b);
        }

        [TestMethod, TestCategory("a7tinfo")]
        public void a7tinfo_DecompressAndInterpret()
        {
            String UNITTEST_EXPECTED_SUBDIR = "a7tinfo";
            String COMPARE_INTERPRETED_FILE = "interpreted.xml";
            String INPUT_FILENAME = "testfile.a7tinfo";
            String INTERPRETER_FILENAME = "a7tinfo.xml";

            String INTERPRETER_PATH = Path.Combine(Folders.UNITTEST_INTERPRETER_DIR, INTERPRETER_FILENAME);
            var COMPARE_PATH = Path.Combine(Folders.UNITTEST_FILE_DIR, Folders.UNITTEST_FILE_EXPECTED_DIR, UNITTEST_EXPECTED_SUBDIR, COMPARE_INTERPRETED_FILE);
            var INPUT_PATH = Path.Combine(Folders.UNITTEST_FILE_DIR, Folders.UNITTEST_FILE_TESTFILES_DIR, UNITTEST_EXPECTED_SUBDIR, INPUT_FILENAME);

            var Interpr = new Interpreter(Interpreter.ToInterpreterDoc(INTERPRETER_PATH));

            Assert.IsTrue( Test_Interpret(INPUT_PATH, COMPARE_PATH, Interpr) );
        }

        #endregion


        public bool Test_Decompress(String FileIn, String FileDecompressed)
        {
            var doc = reader.ReadFile(FileIn);
            using (MemoryStream ms = new MemoryStream())
            {
                doc.Save(ms);
                return FilesAreEqual(ms, FileDecompressed);
            }
        }

        public bool Test_Interpret(String FileIn, String FileInterpreted, Interpreter Interpr)
        {
            var doc = reader.ReadFile(FileIn);
            var interpreted = interpreter.Interpret(doc, Interpr);
            using (MemoryStream ms = new MemoryStream())
            {
                interpreted.Save(ms);
                return FilesAreEqual(ms, FileInterpreted);
            }
        }

        public bool Test_Reinterpret(String FileIn, String FileReinterpret)
        {
            throw new NotImplementedException(); 
        }

        public bool Test_Recompress(String FileIn, String FileRecompressed)
        {
            throw new NotImplementedException();
        }

        public bool FilesAreEqual(MemoryStream StreamToCheck, String FileCompare)
        {
            using (FileStream Compare = File.OpenRead(FileCompare))
            {
                //setup streams!!!
                Compare.Position = 0;
                StreamToCheck.Position = 0;

                if (Compare.Length == StreamToCheck.Length)
                {
                    //we only need one check because we prechecked for equal length here
                    while (Compare.Position < Compare.Length - 1)
                    {
                        if (Compare.ReadByte() != StreamToCheck.ReadByte())
                        {
                            return false; 
                        }
                    }
                    return true; 
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
