using FileDBReader.src;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileDBReader
{
    class internaltest {

        static String TEST_DIRECTORY_NAME = "tests";
        public static void Main(String[] args)
        {
            CttTest();
        }

        public static void ListTest() {
            var reader = new FileReader();
            var exporter = new XmlExporter();
            var writer = new FileWriter();
            var interpreter = new XmlInterpreter();

            const String DIRECTORY_NAME = "lists";
            const String INTERPRETER_FILE = "FileFormats/Island_Gamedata.xml";

            String TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "gamedata_og.data");
            String DECOMPRESSED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "gamedata_decompressed.xml");
            String INTERPRETED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "gamedata_interpreted.xml");
            String TOHEX_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "gamedata_backtohex.xml");
            String EXPORTED_TESTFILE = Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "gamedata.data");

            //decompress gamedata.data
            var interpreterDoc = new XmlDocument();
            interpreterDoc.Load(INTERPRETER_FILE);

            //decompress
            var decompressed = reader.ReadFile(TESTFILE);
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

        public static void CttTest() {
            const String DIRECTORY_NAME = "ctt";
            const String INTERPRETER_FILE = "FileFormats/ctt.xml";

            var zlib = new ZlibFunctions();
            var reader = new FileReader();
            var writer = new FileWriter();
            var interpreter = new XmlInterpreter(); 
            
            var interpreterDoc = new XmlDocument();
            interpreterDoc.Load(INTERPRETER_FILE);

            FileStream fs = File.OpenRead(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "0x1.ctt"));

            //Ubisoft uses 8 magic bytes at the start
            var doc = interpreter.Interpret(reader.ReadSpan(zlib.Decompress(fs, 8)), interpreterDoc);
            doc.Save(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "interpreted.xml"));

        }

        public static void zlibTest() {
            const String DIRECTORY_NAME = "zlib";

            var zlib = new ZlibFunctions();
            var reader = new FileReader();
            var writer = new FileWriter();

            FileStream fs = File.OpenRead(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "0x1.ctt"));

            //Ubisoft uses 8 magic bytes at the start
            var doc = reader.ReadSpan(zlib.Decompress(fs, 8));
            doc.Save(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "decompressed.xml"));

            var Stream = writer.Export(doc, ".bin");
            File.WriteAllBytes(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "shittycompress.ctt"), zlib.Compress(Stream, 1));
        }

        public static void InnerFileDBTest() {
            const String DIRECTORY_NAME = "filedb";

            var reader = new FileReader();
            reader.ReadFile(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "original.bin")).Save(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "original.xml"));
            reader.ReadFile(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "recompressed.bin")).Save(Path.Combine(TEST_DIRECTORY_NAME, DIRECTORY_NAME, "recompressed.xml"));
        }

        public static void CompressionTest() {
            var reader = new FileReader();
            var exporter = new XmlExporter();
            var writer = new FileWriter();
            var interpreter = new XmlInterpreter();

            const String DIRECTORY_NAME = "filedb";
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
            var decompressed = reader.ReadFile(TESTFILE);
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
    }
    
}
