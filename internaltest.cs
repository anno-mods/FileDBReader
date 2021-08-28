using FileDBReader.src;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileDBReader
{
    class internaltest {

        static String TEST_DIRECTORY_NAME = "tests";
        static String FILEFORMAT_DIRECTORY_NAME = "FileFormats";

        static FileReader reader = new FileReader();
        static XmlExporter exporter = new XmlExporter();
        static FileWriter writer = new FileWriter();
        static XmlInterpreter interpreter = new XmlInterpreter();
        static ZlibFunctions zlib = new ZlibFunctions();

        public static void Main(String[] args)
        {
            InfotipTestNewFileVersion();
            A7TINFOTest();
        }

        //Generic Test (TestDirectoryName, InterpreterFileName, TestfileFilename)

        public static void InfotipTestNewFileVersion()
        {
            GenericTest("infotip", "infotip.xml", "export.bin", 2);
        }

        public static void A7TINFOTest()
        {
            GenericTest("a7tinfo", "a7tinfo.xml", "moderate_atoll_ll_01.a7tinfo", 1 );
        }

        public static void ListTest()
        {
            GenericTest("lists", "Island_Gamedata.xml", "gamedata_og.data", 1);
        }

        /// <summary>
        /// Test for the two island interpreters
        /// </summary>
        public static void IslandTest() {
            IslandTestGamedata();
            IslandTestRd3d();
            IslandTestTMC();
        }


        private static void IslandTestGoodwill()
        {
            //test directory
            String DIRECTORY_NAME = "goodwill";
            //interpreter file path
            String INTERPRETER_GAMEDATA = Path.Combine(FILEFORMAT_DIRECTORY_NAME, "internalfiledbtest.xml");
            //input file path
            String GAMEDATA_FILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "gamedata_og.data");
            //output file path
            String GAMEDATA_INTERPRETED_PATH = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "Island_Gamedata_interpreted.xml");
            String GAMEDATA_READ_PATH = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "Island_Gamedata_Read.xml");
            String GAMEDATA_EXPORTED_PATH = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "Island_Gamedata_exported.xml");
            //create interpreter document
            var GamedataInterpreter = new XmlDocument();
            GamedataInterpreter.Load(INTERPRETER_GAMEDATA);

            //decompress interpret and save gamedata.data
            var doc = reader.ReadFile(GAMEDATA_FILE, 1);
            doc.Save(GAMEDATA_READ_PATH);
            var interpreted_gamedata = interpreter.Interpret(reader.ReadFile(GAMEDATA_FILE, 1), GamedataInterpreter);
            interpreted_gamedata.Save(GAMEDATA_INTERPRETED_PATH);

            var exported = exporter.Export(interpreted_gamedata, GamedataInterpreter);
            exported.Save(GAMEDATA_EXPORTED_PATH);



        }

        private static void IslandTestGamedata() {
            //test directory
            String DIRECTORY_NAME = "island";
            //interpreter file path
            String INTERPRETER_GAMEDATA = Path.Combine(FILEFORMAT_DIRECTORY_NAME, "Island_Gamedata.xml");
            //input file path
            String GAMEDATA_FILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "gamedata.data");
            //output file path
            String GAMEDATA_INTERPRETED_PATH = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "Island_Gamedata_interpreted.xml");
            //create interpreter document
            var GamedataInterpreter = new XmlDocument();
            GamedataInterpreter.Load(INTERPRETER_GAMEDATA);

            //decompress interpret and save gamedata.data
            var interpreted_gamedata = interpreter.Interpret(reader.ReadFile(GAMEDATA_FILE, 1), GamedataInterpreter);
            interpreted_gamedata.Save(GAMEDATA_INTERPRETED_PATH);
        }

        private static void IslandTestTMC() 
        {
            //test directory
            String DIRECTORY_NAME = "island";
            //interpreter file path
            String INTERPRETER_GAMEDATA = Path.Combine(FILEFORMAT_DIRECTORY_NAME, "tmc.xml");
            //input file path
            String GAMEDATA_FILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "0x0.tmc");
            //output file path
            String GAMEDATA_INTERPRETED_PATH = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "0x0.xml");
            //create interpreter document
            var GamedataInterpreter = new XmlDocument();
            GamedataInterpreter.Load(INTERPRETER_GAMEDATA);

            //decompress interpret and save gamedata.data
            var interpreted_gamedata = interpreter.Interpret(reader.ReadFile(GAMEDATA_FILE, 1), GamedataInterpreter);
            interpreted_gamedata.Save(GAMEDATA_INTERPRETED_PATH);
        }

        private static void IslandTestRd3d()
        {
            //test directory
            String DIRECTORY_NAME = "island";
            //interpreter file path
            String INTERPRETER_RD3D = Path.Combine(FILEFORMAT_DIRECTORY_NAME, "Island_Rd3d.xml");
            //input file path
            String RD3D_FILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "rd3d.data");
            //output file path
            String RD3D_INTERPRETED_PATH = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "Island_Rd3d_interpreted.xml");
            //create interpreter document
            var GamedataInterpreter = new XmlDocument();
            GamedataInterpreter.Load(INTERPRETER_RD3D);

            //decompress interpret and save the resulting document
            var interpreted_gamedata = interpreter.Interpret(reader.ReadFile(RD3D_FILE, 1), GamedataInterpreter);
            interpreted_gamedata.Save(RD3D_INTERPRETED_PATH);
        }

        /// <summary>
        /// Test for the ctt interpreter.
        /// </summary>
        public static void CttTest() {
            const String DIRECTORY_NAME = "ctt";
            const String INTERPRETER_FILE = "FileFormats/ctt.xml";
            
            var interpreterDoc = new XmlDocument();
            interpreterDoc.Load(INTERPRETER_FILE);

            FileStream fs = File.OpenRead(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "0x1.ctt"));

            //Ubisoft uses 8 magic bytes at the start
            var doc = interpreter.Interpret(reader.ReadSpan(zlib.Decompress(fs, 8), 1), interpreterDoc);
            doc.Save(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "interpreted.xml"));

        }

        /// <summary>
        /// Test for DEFLATE/zlib implementation. 
        /// decompresses 0x1.ctt, ignoring the 8 magic bytes at the start, writes the result to decompressed.xml, then compresses it back.
        /// </summary>
        public static void zlibTest() {
            const String DIRECTORY_NAME = "zlib";

            FileStream fs = File.OpenRead(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "data.a7s"));

            //Ubisoft uses 8 magic bytes at the start
            var doc = reader.ReadSpan(zlib.Decompress(fs, 0), 1);
            doc.Save(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "decompressed.xml"));

            var Stream = writer.Export(doc, ".bin");
            File.WriteAllBytes(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "shittycompress.ctt"), zlib.Compress(Stream, 1));
        }

        /// <summary>
        /// decompresses the two files original.bin and recompressed.bin which are preextracted inner filedb files from gamedata_og.data
        /// </summary>
        public static void InnerFileDBTest() {
            const String DIRECTORY_NAME = "filedb";

            var reader = new FileReader();
            reader.ReadFile(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "original.bin"), 1).Save(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "original.xml"));
            reader.ReadFile(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "recompressed.bin"), 1).Save(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "recompressed.xml") );
        }

        /// <summary>
        /// Decompresses gamedata_og.data, inteprets it with internalfiledbtest.xml, converts it back to hex using the same interpreter, exports back to filedb compression. 
        /// A file is saved at each stage of the process.
        /// </summary>
        public static void CompressionTest() {

            const String DIRECTORY_NAME = "innerfiledb";
            const String INTERPRETER_FILE = "FileFormats/internalfiledbtest.xml";

            String TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "gamedata_og.data");
            String DECOMPRESSED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "gamedata_decompressed.xml");
            String INTERPRETED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "gamedata_interpreted.xml");
            String TOHEX_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "gamedata_backtohex.xml");
            String EXPORTED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "gamedata.data");

            //decompress gamedata.data
            var interpreterDoc = new XmlDocument();
            interpreterDoc.Load(INTERPRETER_FILE);

            //decompress
            var decompressed = reader.ReadFile(TESTFILE, 1);
            decompressed.Save(DECOMPRESSED_TESTFILE);

            //interpret
            var interpreted = interpreter.Interpret(decompressed, interpreterDoc);
            interpreted.Save(INTERPRETED_TESTFILE);

            //to hex 
            var Hexed = exporter.Export(interpreted, interpreterDoc);
            Hexed.Save(TOHEX_TESTFILE);

            //back to gamedata 
            writer.Export(Hexed, EXPORTED_TESTFILE);
        }


        public static bool FilesAreEqual(FileInfo first, FileInfo second)
        {
            const int BYTES_TO_READ = sizeof(Int64);

            if (first.Length != second.Length)
                return false;

            if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
                return true;

            int iterations = (int)Math.Ceiling((double)first.Length / BYTES_TO_READ);

            using (FileStream fs1 = first.OpenRead())
            using (FileStream fs2 = second.OpenRead())
            {
                byte[] one = new byte[BYTES_TO_READ];
                byte[] two = new byte[BYTES_TO_READ];

                for (int i = 0; i < iterations; i++)
                {
                    fs1.Read(one, 0, BYTES_TO_READ);
                    fs2.Read(two, 0, BYTES_TO_READ);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return false;
                }
            }

            return true;
        }
        public static void GenericTest(String DIRECTORY_NAME, String INTERPREFER_FILE_NAME, String TESTFILE_NAME, int FileVersion)
        {
            String INTERPRETER_FILE = Path.Combine(FILEFORMAT_DIRECTORY_NAME, INTERPREFER_FILE_NAME);
            String TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME);

            String DECOMPRESSED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_decompressed.xml");
            String INTERPRETED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_interpreted.xml");
            String TOHEX_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_recompressed.xml");
            String EXPORTED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_converted" + Path.GetExtension(TESTFILE_NAME));

            //decompress gamedata.data
            var interpreterDoc = new XmlDocument();
            interpreterDoc.Load(INTERPRETER_FILE);

            //decompress
            var decompressed = reader.ReadFile(TESTFILE, FileVersion);//interpret
            decompressed.Save(DECOMPRESSED_TESTFILE);
            var interpreted = interpreter.Interpret(decompressed, interpreterDoc);
            interpreted.Save(INTERPRETED_TESTFILE);

            //to hex 
            var Hexed = exporter.Export(interpreted, interpreterDoc);
            Hexed.Save(TOHEX_TESTFILE);

            //back to gamedata 
            writer.Export(Hexed, EXPORTED_TESTFILE);

            var OriginalInfo = new FileInfo(TESTFILE);
            var DecompressedInfo = new FileInfo(DECOMPRESSED_TESTFILE);
            var InterpretedInfo = new FileInfo(INTERPRETED_TESTFILE);
            var RehexedInfo = new FileInfo(TOHEX_TESTFILE);
            var ReexportedInfo = new FileInfo(EXPORTED_TESTFILE);

            Console.WriteLine("File Test: {0}", TESTFILE_NAME);
            Console.WriteLine("Used FileDBCompression Version: {0}", FileVersion);
            Console.WriteLine("FILEDB FILES FILESIZE\nOriginal: {0}, Converted: {1}. Filesize Equality:{2}", OriginalInfo.Length, ReexportedInfo.Length, OriginalInfo.Length == ReexportedInfo.Length);
            //This check will probably give a false if there is internal compression!
            Console.WriteLine("XML FILES FILESIZE\n Decompressed: {0}, Recompressed: {1}. Filesize Equality: {2}", OriginalInfo.Length, ReexportedInfo.Length, OriginalInfo.Length == ReexportedInfo.Length);

            Console.WriteLine("File Test Done");
            Console.WriteLine("--------------------------------------------------");
        }

        private static byte[] GetFileHash(String FileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(FileName))
                {
                    return md5.ComputeHash(stream);
                }
            }
        }
    }
    
}
