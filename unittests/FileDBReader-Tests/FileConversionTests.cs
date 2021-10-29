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
            String INPUT_FILENAME = "version1.tmc";
            var STRING_PATH = Path.Combine(Folders.UNITTEST_FILE_DIR, Folders.UNITTEST_FILE_TESTFILES_DIR, Folders.UNITTEST_VERSION_SUBDIR, INPUT_FILENAME);
            Assert.IsTrue(reader.CheckFileVersion(STRING_PATH) == 1);
        }

        [TestMethod, TestCategory("version")]
        public void Detects_Version2()
        {
            String INPUT_FILENAME = "version2.bin";
            var STRING_PATH = Path.Combine(Folders.UNITTEST_FILE_DIR, Folders.UNITTEST_FILE_TESTFILES_DIR, Folders.UNITTEST_VERSION_SUBDIR, INPUT_FILENAME);
            Assert.IsTrue(reader.CheckFileVersion(STRING_PATH) == 2);
        }
        #endregion

        //----------------------------#######-------------------------------------//
        // The following tests prove:

        // <Base File> -> Decompression is also validated.
        // <Base File> -> Decompression -> Recompression -> <Recompressed File> will lead to <Base File> <=> <Recompressed File>
        // <Decompressed File> -> Interpretation -> Reinterpretation -> <Reinterpreted File> will lead to <Decompressed File> <=> <Reinterpreted File>. 
        //Equivalence is proven by expected preconverted files that have been deemed valid by human input.

        //----------------------------#######-------------------------------------//

        #region A7TINFO_TEST

        [TestMethod, TestCategory("a7tinfo")]
        public void a7tinfo_Decompress()
        {
            String COMPARE_DECOMPRESSED_FILE = "decompressed.xml";
            String INPUT_FILE = "testfile.a7tinfo";

            Assert.IsTrue(Test_Decompress(Folders.UNITTEST_A7TINFO_SUBDIR, INPUT_FILE, COMPARE_DECOMPRESSED_FILE, out var decompressed));
        }

        [TestMethod, TestCategory("a7tinfo")]
        public void a7tinfo_FileEquality_AfterCompressionCycle()
        {
            String COMPARE_RECOMPRESSED_FILE = "recompressed.a7tinfo";
            String INPUT_FILE = "testfile.a7tinfo";
            int COMPRESSION_VERSION = 1;

            Assert.IsTrue(Test_DecompressAndRecompress(Folders.UNITTEST_A7TINFO_SUBDIR, INPUT_FILE, COMPARE_RECOMPRESSED_FILE, COMPRESSION_VERSION));
        }

        [TestMethod, TestCategory("a7tinfo")]
        public void a7tinfo_FileEquality_AfterInterpretation()
        {
            String COMPARE_REINTERPRETED_FILE = "reinterpreted.xml";
            String INPUT_FILE = "decompressed.xml";
            String INTERPRETER_FILE = "a7tinfo.xml";

            Assert.IsTrue(Test_InterpretAndReinterpret(Folders.UNITTEST_A7TINFO_SUBDIR, INPUT_FILE, COMPARE_REINTERPRETED_FILE, INTERPRETER_FILE));
        }

        #endregion

        #region TMC_TEST
        [TestMethod, TestCategory("tmc")]
        public void tmc_FileEquality_AfterCompressionCycle()
        {
            String COMPARE_RECOMPRESSED_FILE = "recompressed.tmc";
            String INPUT_FILE = "testfile.tmc";
            int COMPRESSION_VERSION = 1;

            Assert.IsTrue(Test_DecompressAndRecompress(Folders.UNITTEST_TMC_SUBDIR, INPUT_FILE, COMPARE_RECOMPRESSED_FILE, COMPRESSION_VERSION));
        }

        [TestMethod, TestCategory("a7tinfo")]
        public void tmc_FileEquality_AfterInterpretation()
        {
            String COMPARE_REINTERPRETED_FILE = "reinterpreted.xml";
            String INPUT_FILE = "decompressed.xml";
            String INTERPRETER_FILE = "tmc.xml";

            Assert.IsTrue(Test_InterpretAndReinterpret(Folders.UNITTEST_TMC_SUBDIR, INPUT_FILE, COMPARE_REINTERPRETED_FILE, INTERPRETER_FILE));
        }

        [TestMethod, TestCategory("a7tinfo")]
        public void tmc_Decompress()
        {
            String COMPARE_DECOMPRESSED_FILE = "decompressed.xml";
            String INPUT_FILE = "testfile.tmc";

            Assert.IsTrue(Test_Decompress(Folders.UNITTEST_TMC_SUBDIR, INPUT_FILE, COMPARE_DECOMPRESSED_FILE, out var decompressed));
        }


        #endregion

        #region GenericTests

        /// <summary>
        /// Tests whether a file stays the same after decompressing and recompressing.
        /// </summary>
        /// <param name="FileIn"></param>
        /// <param name="FileRecompressed"></param>
        /// <returns></returns>
        public bool Test_DecompressAndRecompress(String UnittestSubdir, String FileIn, String ExpectedResult, int CompressionVersion)
        {
            var COMPARE_EXPECTED_PATH = Path.Combine(Folders.UNITTEST_FILE_DIR, Folders.UNITTEST_FILE_EXPECTED_DIR, UnittestSubdir, ExpectedResult);
            var INPUT_PATH = Path.Combine(Folders.UNITTEST_FILE_DIR, Folders.UNITTEST_FILE_TESTFILES_DIR, UnittestSubdir, FileIn);
            
            var decompressed = reader.ReadFile(INPUT_PATH);

            using (MemoryStream result = new MemoryStream())
            using (FileStream expected = File.OpenRead(COMPARE_EXPECTED_PATH))
            {
                var recompressed = writer.Export(decompressed, result, CompressionVersion);
                return StreamsAreEqual(result, expected);
            }
        }

        public bool Test_InterpretAndReinterpret(String UnittestSubdir, String FileIn, String ExpectedResult, String InterpreterFile)
        {
            var EXPECTED_PATH = Path.Combine(Folders.UNITTEST_FILE_DIR, Folders.UNITTEST_FILE_EXPECTED_DIR, UnittestSubdir, ExpectedResult);

            //note: Decompressed File is taken from the expected directory to avoid duplication.
            var INPUT_PATH = Path.Combine(Folders.UNITTEST_FILE_DIR, Folders.UNITTEST_FILE_EXPECTED_DIR, UnittestSubdir, FileIn);

            String INTERPRETER_PATH = Path.Combine(Folders.UNITTEST_INTERPRETER_DIR, InterpreterFile);

            var Interpr = new Interpreter(Interpreter.ToInterpreterDoc(INTERPRETER_PATH));
            var DecompressedDocument = new XmlDocument();
            DecompressedDocument.Load(INPUT_PATH);
            var ExpectedDocument = new XmlDocument();
            ExpectedDocument.Load(EXPECTED_PATH);

            //execute
            var interpretedDocument = interpreter.Interpret(DecompressedDocument, Interpr);
            var reinterpretedDocument = exporter.Export(interpretedDocument, Interpr);

            using (MemoryStream ExpectedResultStream = new MemoryStream())
            using (MemoryStream ReinterpretedStream = new MemoryStream())
            {
                ExpectedDocument.Save(ExpectedResultStream);
                reinterpretedDocument.Save(ReinterpretedStream);
                //Return
                return StreamsAreEqual(ExpectedResultStream, ReinterpretedStream);
            }
        }

        public bool Test_Decompress(String UnittestSubdir, String FileIn, String ExpectedResult, out XmlDocument decompressedResult)
        {
            var COMPARE_PATH = Path.Combine(Folders.UNITTEST_FILE_DIR, Folders.UNITTEST_FILE_EXPECTED_DIR, UnittestSubdir, ExpectedResult);
            var INPUT_PATH = Path.Combine(Folders.UNITTEST_FILE_DIR, Folders.UNITTEST_FILE_TESTFILES_DIR, UnittestSubdir, FileIn);

            var decompressed = reader.ReadFile(INPUT_PATH);
            decompressedResult = decompressed;
            using (MemoryStream ms = new MemoryStream())
            using (FileStream expected = File.OpenRead(COMPARE_PATH))
            {
                decompressed.Save(ms);
                return StreamsAreEqual(ms, expected);
            }
        }

        public bool Test_Interpret(String DecompressedFile, String ExpectedResult, String InterpreterFile, out XmlDocument interpretedResult)
        {
            //setup
            String INTERPRETER_PATH = Path.Combine(Folders.UNITTEST_INTERPRETER_DIR, InterpreterFile);
            var Interpr = new Interpreter(Interpreter.ToInterpreterDoc(INTERPRETER_PATH));
            var Decompressed = new XmlDocument();
            Decompressed.Load(DecompressedFile);

            //Execute
            var interpreted = interpreter.Interpret(Decompressed, Interpr);
            interpretedResult = interpreted; 

            using (MemoryStream result = new MemoryStream())
            using (FileStream expected = File.OpenRead(ExpectedResult))
            {
                interpreted.Save(result);
                return StreamsAreEqual(result, expected);
            }
        }

        public bool Test_Reinterpret(XmlDocument Interpreted, String FileInterpreted, String InterpreterFile, out XmlDocument reinterpretedResult)
        {
            String INTERPRETER_PATH = Path.Combine(Folders.UNITTEST_INTERPRETER_DIR, InterpreterFile);
            var Interpr = new Interpreter(Interpreter.ToInterpreterDoc(INTERPRETER_PATH));

            var reinterpreted = exporter.Export(Interpreted, Interpr);
            reinterpretedResult = reinterpreted;

            using (MemoryStream ms = new MemoryStream())
            using (FileStream expected = File.OpenRead(FileInterpreted))
            {
                reinterpreted.Save(ms);
                return StreamsAreEqual(ms, expected);
            }
        }

        public bool Test_Recompress(XmlDocument Reinterpreted, String FileRecompressed, int CompressionVersion)
        {
            using (MemoryStream TargetStream = new MemoryStream())
            using (FileStream expected = File.OpenRead(FileRecompressed))
            {
                writer.Export(Reinterpreted, TargetStream, CompressionVersion);
                return StreamsAreEqual(TargetStream, expected);
            }
        }

        #endregion

        #region HelpfulFunctions

        public bool StreamsAreEqual(Stream StreamToCheck, Stream StreamCompare)
        {
            StreamToCheck.Position = 0;
            StreamCompare.Position = 0;

            if (StreamCompare.Length == StreamToCheck.Length)
            {
                //we only need one check because we prechecked for equal length here
                while (StreamCompare.Position < StreamCompare.Length - 1)
                {
                    if (StreamCompare.ReadByte() != StreamToCheck.ReadByte())
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
        #endregion
    }
}
