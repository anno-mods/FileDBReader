using FileDBReader.src;
using FileDBReader.src.XmlRepresentation;
using FileDBSerializing;
using FileDBSerializing.LookUps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Xml;

namespace FileDBReader
{
    class DevPlayground {

        static String TEST_DIRECTORY_NAME = "dev_files";
        static String FILEFORMAT_DIRECTORY_NAME = "FileFormats";

        //FileDB
        static XmlExporter exporter = new XmlExporter();
        static XmlInterpreter interpreter = new XmlInterpreter();
        static ZlibFunctions zlib = new ZlibFunctions();

        static Reader reader = new Reader();
        static Writer writer = new Writer();

        //Fc Files
        static FcFileHelper FcFileHelper = new FcFileHelper();

        public static void Main(String[] args)
        {
            /*
            var deserializer = new FileDBDeserializer<FileDBDocument_V1>();
            var V2Document = deserializer.Deserialize("dev_files/island/0x0.tmc");

            FileDBSerializer serializer = new FileDBSerializer();
            var outstream = serializer.Serialize(V2Document, new MemoryStream());

            Stopwatch watch = new Stopwatch();
            watch.Start(); 
            var filestream = File.Create("version2.bin");
            outstream.Position = 0;
            outstream.CopyTo(filestream);
            filestream.Close();
            watch.Stop();
            Console.WriteLine("File Writing Operation took: {0} ms", watch.Elapsed.TotalMilliseconds);

            Console.WriteLine("Finished Test File: file.db");
            */
        }

        private static void NodelookupTest()
        {
            DocumentParser ser = new DocumentParser(FileDBDocumentVersion.Version1);

            var fdoc = ser.LoadFileDBDocument(File.OpenRead("dev_files/a7tinfo/moderate_atoll_ll_01.a7tinfo"));
            fdoc.SelectNodes("MapTemplate/Size");
        }

        #region GenericTestFcFile

        public static void FcFileDevTest()
        {
            FcFile_GenericTest("residence_tier02_estate01.fc");
            FcFile_GenericTest("world_map_01.fc");
            FcFile_GenericTest("food_07.fc");
            FcFile_GenericTest("mining_08.fc");
            FcFile_GenericTest("workshop_06.fc");
            FcFile_GenericTest("electricity_01.fc");
        }
        #endregion

        #region GenericTestInstancesFileDB

        //Generic Test(TestDirectoryName, InterpreterFileName, TestfileFilename, FileVersion)

        /// <summary>
        /// Test for the two island interpreters
        /// </summary>
        /// 

        public static void GGJTest()
        {
            InvalidTagNameHelper.AddReplaceOp("Bus Activation", "BusActivation");
            GenericTest("GGJ", "Island_Gamedata_V2.xml", "gamedata.data", 2);
            InvalidTagNameHelper.RemoveReplaceOp("BusActivation");
        }

        public static void BrokenTagsTest()
        {
            GenericTest("BrokenTags", "Island_Gamedata.xml", "gamedata.data", 1);
        }

        public static void IslandTest()
        {
            IslandTestGamedata();
            IslandTestRd3d();
            IslandTestTMC();
        }

        private static void IslandTestGamedata()
        {
            GenericTest("island", "Island_Gamedata.xml", "gamedata.data", 1);
        }

        private static void IslandTestTMC()
        {
            GenericTest("island", "tmc.xml", "0x0.tmc", 1);
        }

        private static void IslandTestRd3d()
        {
            GenericTest("island", "Island_Rd3d.xml", "rd3d.data", 1);
        }

        private static void MapGamedataTest()
        {
            GenericTest("maps", "map_Gamedata.xml", "gamedata.data", 1);
        }

        public static void InfotipTestNewFileVersion()
        {
            GenericTest("infotip", "infotip.xml", "export.bin", 2);
        }

        public static void A7TINFOTest()
        {
            GenericTest("a7tinfo", "a7tinfo.xml", "moderate_atoll_ll_01.a7tinfo", 1);
        }

        public static void ListTest()
        {
            GenericTest("lists", "Island_Gamedata.xml", "gamedata_og.data", 1);
        }

        public static void RenamedTagsTest()
        {
            Dictionary<string, string> Renames = new Dictionary<string, string>();
            Renames.Add("Delayed Construction", "DelayedConstruction");
            InvalidTagNameHelper.ReplaceOperations = Renames; 
            GenericTest("RenamedTags", "Island_Gamedata.xml", "gamedata.data", 1);
            InvalidTagNameHelper.Reset(); 
        }

        #endregion

        #region FileDBTests

        /// <summary>
        /// Test for the ctt interpreter.
        /// </summary>
        /// 


        /// <summary>
        /// Test for DEFLATE/zlib implementation. 
        /// decompresses 0x1.ctt, ignoring the 8 magic bytes at the start, writes the result to decompressed.xml, then compresses it back.
        /// </summary>
        
        
        /*public static void zlibTest() {
            const String DIRECTORY_NAME = "zlib";

            FileStream fs = File.OpenRead(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "0x1.ctt"));

            //Ubisoft uses 8 magic bytes at the start
            var doc = reader.Read(zlib.Decompress(fs, 8));
            doc.Save(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "decompressed.xml"));
        }*/

        public static void GenericTest(String DIRECTORY_NAME, String INTERPREFER_FILE_NAME, String TESTFILE_NAME, int FileVersion)
        {
            String INTERPRETER_FILE = Path.Combine(FILEFORMAT_DIRECTORY_NAME, INTERPREFER_FILE_NAME);
            String TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME);

            String DECOMPRESSED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, Path.GetFileNameWithoutExtension(TESTFILE_NAME) + "_decompressed.xml");
            String INTERPRETED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, Path.GetFileNameWithoutExtension(TESTFILE_NAME) + "_interpreted.xml");
            String TOHEX_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, Path.GetFileNameWithoutExtension(TESTFILE_NAME) + "_reinterpreted.xml");
            String EXPORTED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, Path.GetFileNameWithoutExtension(TESTFILE_NAME) + "_recompressed" + Path.GetExtension(TESTFILE_NAME));

            Console.WriteLine("File Test: {0}", TESTFILE_NAME);

            //decompress
            var decompressed = reader.Read(File.OpenRead(TESTFILE));
            decompressed.Save(DECOMPRESSED_TESTFILE);

            var interpreted = interpreter.Interpret(decompressed, new Interpreter(Interpreter.ToInterpreterDoc(INTERPRETER_FILE)));
            interpreted.Save(INTERPRETED_TESTFILE);

            //to hex 
            var Hexed = exporter.Export(interpreted, new Interpreter(Interpreter.ToInterpreterDoc(INTERPRETER_FILE)));
            Hexed.Save(TOHEX_TESTFILE);

            //back to gamedata 
            writer.Write(Hexed, File.Create(EXPORTED_TESTFILE), FileVersion);

            var OriginalInfo = new FileInfo(TESTFILE);
            var DecompressedInfo = new FileInfo(DECOMPRESSED_TESTFILE);
            var InterpretedInfo = new FileInfo(INTERPRETED_TESTFILE);
            var RehexedInfo = new FileInfo(TOHEX_TESTFILE);
            var ReexportedInfo = new FileInfo(EXPORTED_TESTFILE);

            
            Console.WriteLine("Used FileDBCompression Version for re-export: {0}", FileVersion);
            Console.WriteLine("FILEDB FILES FILESIZE\nOriginal: {0}, Converted: {1}. Filesize Equality:{2}", OriginalInfo.Length, ReexportedInfo.Length, OriginalInfo.Length == ReexportedInfo.Length);
            //This check will probably give a false if there is internal compression!
            Console.WriteLine("XML FILES FILESIZE\n Decompressed: {0}, Recompressed: {1}. Filesize Equality: {2}", DecompressedInfo.Length, RehexedInfo.Length, DecompressedInfo.Length == RehexedInfo.Length);

            Console.WriteLine("File Test Done");
            Console.WriteLine("--------------------------------------------------");
        }
        #endregion

        #region FCTests

        public static void ClosingFileTest()
        {
            FcFile_GenericTest("fcFiles", "FcFile.xml", "cannon_ball_small_01.rdp");
        }

        public static void FcFile_GenericTest(String TESTFILE_NAME)
        {
            FcFile_GenericTest("fcFiles", "FcFile.xml", TESTFILE_NAME);
        }
        public static void FcFile_GenericTest(String DIRECTORY_NAME, String INTERPREFER_FILE_NAME, String TESTFILE_NAME)
        {
            String TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME);

            String CDATAREAD_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_CdataRead.xml");
            String INTERPRETED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_interpreted.xml");
            String REINTERPRETED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_reinterpreted.xml");
            String CDATAWRITTEN_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, TESTFILE_NAME + "_CdataWritten" + Path.GetExtension(TESTFILE_NAME));

            
            
            String INTERPRETER_FILE = Path.Combine(FILEFORMAT_DIRECTORY_NAME, INTERPREFER_FILE_NAME);
            var interpreterDoc = new XmlDocument();
            interpreterDoc.Load(INTERPRETER_FILE);

            //read
            var Read = FcFileHelper.ReadFcFile(SecureIoHandler.ReadHandle(TESTFILE));
            Read.Save(CDATAREAD_TESTFILE);

            var Interpreted = interpreter.Interpret(Read, new Interpreter(Interpreter.ToInterpreterDoc(INTERPRETER_FILE)));
            Interpreted.Save(INTERPRETED_TESTFILE);

            var Reinterpreted = exporter.Export(Interpreted, new Interpreter(Interpreter.ToInterpreterDoc(INTERPRETER_FILE)));
            Reinterpreted.Save(REINTERPRETED_TESTFILE);

            var Written = FcFileHelper.ConvertFile(FcFileHelper.XmlFileToStream(Reinterpreted), ConversionMode.Write, new MemoryStream());
            Save(Written, CDATAWRITTEN_TESTFILE);

            //save 
            //var stream = FcFileHelper.ConvertFile(File.OpenRead(TESTFILE), ConversionMode.Read);
            //var outstream = FcFileHelper.ConvertFile(stream, ConversionMode.Write);
            
            //ShowFileWithDefaultProgram(CDATAWRITTEN_TESTFILE);

            try
            {
                var OriginalInfo = new FileInfo(TESTFILE);
                var DecompressedInfo = new FileInfo(CDATAREAD_TESTFILE);
                var InterpretedInfo = new FileInfo(INTERPRETED_TESTFILE);
                var RehexedInfo = new FileInfo(REINTERPRETED_TESTFILE);
                var ReexportedInfo = new FileInfo(CDATAWRITTEN_TESTFILE);

                Console.WriteLine("File Test: {0}", TESTFILE_NAME);
                Console.WriteLine("FC FILES FILESIZE\nOriginal: {0}, Converted: {1}. Filesize Equality:{2}", OriginalInfo.Length, ReexportedInfo.Length, OriginalInfo.Length == ReexportedInfo.Length);
                //This check will probably give a false if there is internal compression!
                Console.WriteLine("XML FILES FILESIZE\n Cdata Read: {0}, Cdata Rewritten: {1}. Filesize Equality: {2}", DecompressedInfo.Length, RehexedInfo.Length, DecompressedInfo.Length == RehexedInfo.Length);

                Console.WriteLine("File Test Done");
                Console.WriteLine("--------------------------------------------------");
            }
            catch (Exception)
            {
                Console.WriteLine("Currently undergoing maintenance, please fuck off");
            }
        }
        #endregion

        #region UniversalMethods
        private static void ShowFileWithDefaultProgram(FileStream f)
        {
            ShowFileWithDefaultProgram(f.Name);
        }

        private static void ShowFileWithDefaultProgram(String Filename)
        {
            using (Process fileopener = new Process())
            {
                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = "\"" + Filename + "\"";
                fileopener.Start();
            }
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

        private static void Save(Stream Stream, String Filename)
        {
            var fs = File.Create(Filename);
            Stream.Position = 0;
            Stream.CopyTo(fs);
            fs.Close();
        }

        #endregion

    }

}
