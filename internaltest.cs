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
        public static void Main(String[] args)
        {
            var reader = new FileReader();
            var exporter = new XmlExporter();
            var writer = new FileWriter();
            var interpreter = new XmlInterpreter();

            String DIRECTORY_NAME = "internaltest/";
            String INTERPRETER_FILE = "FileFormats/internalfiledbtest.xml";
            String TESTFILE = DIRECTORY_NAME + "gamedata_og.data";
            String DECOMPRESSED_TESTFILE = DIRECTORY_NAME + "gamedata_decompressed.xml";
            String INTERPRETED_TESTFILE = DIRECTORY_NAME + "gamedata_interpreted.xml";
            String TOHEX_TESTFILE = DIRECTORY_NAME + "gamedata_backtohex.xml";
            String EXPORTED_TESTFILE = DIRECTORY_NAME + "gamedata.data";

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
