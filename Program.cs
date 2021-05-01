using System;
using System.Collections.Generic;
using System.IO;
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
        #endregion

        #region Methods
        private static void Main(string[] args) {
            var reader = new FileReader();
            var exporter = new XmlExporter();
            var writer = new FileWriter();
            var interpreter = new XmlInterpreter();

            CommandLine.Parser.Default.ParseArguments<DecompressOptions, CompressOptions, InterpretOptions, toHexOptions>(args).MapResult(
                    (DecompressOptions o) =>
                    {
                        foreach (String s in o.InputFiles)
                            reader.ReadFile(s);
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
                            interpreter.Interpret(s, o.Interpreter);
                        return 0;
                    },
                    (toHexOptions o) =>
                    {
                        foreach (String s in o.InputFiles)
                            exporter.Export(s, o.Interpreter);
                        return 0; 
                    },
                    e => 1
                ) ;
        }
        #endregion Methods
    }
}