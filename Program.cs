using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CommandLine; 

namespace FileDBReader {


  internal class Program {

        #region CmdLineOptions
        [Verb("decompress", HelpText = "Decompress a file from filedb to xml. Data will be represented as hex strings.")]
        class DecompressOptions
        {
            [Option('f', "file", Required = true, HelpText = "input files to be decompressed.")]
            public IEnumerable<String> InputFiles { get; set; }
        }

        [Verb("compress", HelpText = "Recompress an xml file to filedb. Requires data to be represented as hex strings")]
        class CompressOptions
        {
            [Option('f', "file", Required = true, HelpText = "input files")]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('o', "outputFileExtension", Required = false, HelpText = "file Format of the output file")]
            public String OutputFileExtension{ get; set; }
        }

        [Verb("interpret", HelpText = "Interpret an xml file that uses hex strings as texts. An interpreter file is needed")]
        class InterpretOptions
        {
            [Option('f', "file", Required = true, HelpText = "input files")]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('i', "interpreter", Required = true, HelpText = "Interpreter file")]
            public String Interpreter { get; set; }
        }

        [Verb("toHex", HelpText = "Convert all text in an xml file to hex. An interpreter file is needed")]
        class toHexOptions
        {
            [Option('f', "file", Required = true, HelpText = "input files")]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('i', "interpreter", Required = true, HelpText = "Interpreter file")]
            public String Interpreter { get; set; }
        }

        [Verb("decompress_interpret", HelpText = "decompress a filedb file and interpret it. An interpreter file is needed")]
        class Decompress_Interpret_Options
        {
            [Option('f', "file", Required = true, HelpText = "input files")]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('i', "interpreter", Required = true, HelpText = "Interpreter file")]
            public String Interpreter { get; set; }
        }
        [Verb("recompress_export", HelpText = "reimport an xml file to filedb. An interpreter file is needed")]
        class Recompress_Export_Options
        {
            [Option('f', "file", Required = true, HelpText = "input files")]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('i', "interpreter", Required = true, HelpText = "Interpreter file")]
            public String Interpreter { get; set; }

            [Option('o', "outputFileExtension", Required = false, HelpText = "file Format of the output file")]
            public String OutputFileExtension { get; set; }
        }
        #endregion

        #region Methods
        private static void Main(string[] args) {
            var reader = new FileReader();
            var exporter = new XmlExporter();
            var writer = new FileWriter();
            var interpreter = new XmlInterpreter();


            //todo make this pretty by adjusting writing to reading flow.
            CommandLine.Parser.Default.ParseArguments<DecompressOptions, CompressOptions, InterpretOptions, toHexOptions, Decompress_Interpret_Options, Recompress_Export_Options>(args).MapResult(
                    (DecompressOptions o) =>
                    {
                        foreach (String s in o.InputFiles) 
                        {
                            var doc = reader.ReadFile(s);
                            doc.Save(Path.ChangeExtension(s, "xml"));
                        }
                        return 0;
                    },
                    (CompressOptions o) =>
                    {
                        var ext = "";
                        if (o.OutputFileExtension != null) {
                            ext = o.OutputFileExtension;
                        }
                        else {
                            ext = ".filedb";
                        }
                        foreach (String s in o.InputFiles)
                            writer.Export(s, ext);
                        return 0;
                    },
                    (InterpretOptions o) =>
                    {
                        foreach (String s in o.InputFiles) 
                        {
                            var doc = interpreter.Interpret(s, o.Interpreter);
                            doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(s, "_interpreted"), "xml"));
                        }
                        return 0;
                    },
                    (toHexOptions o) =>
                    {
                        foreach (String s in o.InputFiles) {
                            var doc = exporter.Export(s, o.Interpreter);
                            doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(s, "_exported"), "xml"));
                        }
                        return 0; 
                    },
                    (Decompress_Interpret_Options o) =>
                    {
                        foreach (String s in o.InputFiles) {
                            var interpreterDoc = new XmlDocument();
                            interpreterDoc.Load(o.Interpreter);
                            var doc = interpreter.Interpret(reader.ReadFile(s), interpreterDoc, s);
                            doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(s, "_d_i"), "xml"));

                        }
                        return 0;
                    },
                    (Recompress_Export_Options o) =>
                    {
                        var ext = "";
                        if (o.OutputFileExtension != null)
                        {
                            ext = o.OutputFileExtension;
                        }
                        else
                        {
                            ext = ".filedb";
                        }
                        foreach (String s in o.InputFiles)
                        {
                            var interpreterDoc = new XmlDocument();
                            interpreterDoc.Load(o.Interpreter);
                            writer.Export(exporter.Export(s, o.Interpreter), o.OutputFileExtension, s);

                        }
                        return 0;
                    },
                    e => 1
                ) ;
        }
        #endregion Methods
    }
}