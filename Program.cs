using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CommandLine;
using FileDBReader.src;

namespace FileDBReader {

  internal class Program {

        //VERB EXPLANATIONS
        private const String DECOMPRESS_EXPL = "Decompress files from filedb compression to xml. Data will be represented as hex strings.";
        private const String COMPRESS_EXPL = "Compress a file from xml to filedb. Expects data to be represented as hex strings.";
        private const String INTERPRET_EXPL = "Interpret an xml file that uses hex strings as texts. An interpreter file is needed";
        private const String REINTERPRET_EXPL = "Convert all text in an xml file to hex. An interpreter file is needed";
        private const String VERSION_EXPL = "checks the compression version of each input file";

        private const String INPUT_FILE_MESSAGE = "Input files";
        private const String COMPRESSION_VERSION_MESSAGE = "File Version: \n1 for Anno 1800 files up to GU12 \n2for Anno 1800 files after GU12";
        private const String INTERPRETER_FILE_HELP = "Interpreter Filepath";
        private const String INTERPRETER_SKIP_HELP = "If provided, decompressing/reinterpreting is automatically done as a step-in-between and the program will print the interpreted/recompressed result.";
        private const String OUTPUT_FILEFORMAT_HELP = "File extension for the output file";

        #region CmdLineOptions
        [Verb("decompress", HelpText = DECOMPRESS_EXPL)]
        class DecompressOptions
        {
            [Option('f', "files", Required = true, HelpText = INPUT_FILE_MESSAGE)]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('c', "CompressionVersion", Required = false, HelpText = COMPRESSION_VERSION_MESSAGE)]
            public int CompressionVersion { get; set; }

            //optional interpreter file. If provided, it will directly interpret the decompressed file. 
            [Option('i', "interpreter", Required = false, HelpText = INTERPRETER_FILE_HELP + ". " + INTERPRETER_SKIP_HELP)]
            public String Interpreter { get; set; }

            [Option('y', "overwrite", Required = false, Default = false)]
            public bool overwrite { get; set; }
        }

        [Verb("compress", HelpText = COMPRESS_EXPL)]
        class CompressOptions
        {
            [Option('f', "files", Required = true, HelpText = INPUT_FILE_MESSAGE)]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('o', "outputFileExtension", Required = false, HelpText = OUTPUT_FILEFORMAT_HELP)]
            public String OutputFileExtension{ get; set; }

            [Option('c', "CompressionVersion", Required = true, HelpText = COMPRESSION_VERSION_MESSAGE)]
            public int CompressionVersion { get; set; }

            //optional interpreter file. If provided, it will go directly from an interpreted file to final result
            [Option('i', "interpreter", Required = false, HelpText = INTERPRETER_FILE_HELP + " | " + INTERPRETER_SKIP_HELP )]
            public String Interpreter { get; set; }

            [Option('y', "overwrite", Required = false, Default = false)]
            public bool overwrite { get; set; }
        }

        [Verb("interpret", HelpText = INTERPRET_EXPL)]
        class InterpretOptions
        {
            [Option('f', "files", Required = true, HelpText = INPUT_FILE_MESSAGE)]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('i', "interpreter", Required = true, HelpText = INTERPRETER_FILE_HELP)]
            public String Interpreter { get; set; }

            [Option('y', "overwrite", Required = false, Default = false)]
            public bool overwrite { get; set; }
        }

        [Verb("toHex", HelpText = REINTERPRET_EXPL)]
        class toHexOptions
        {
            [Option('f', "files", Required = true, HelpText = INPUT_FILE_MESSAGE)]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('i', "interpreter", Required = true, HelpText = INTERPRETER_FILE_HELP)]
            public String Interpreter { get; set; }

            [Option('y', "overwrite", Required = false, Default = false)]
            public bool overwrite { get; set; }
        }

        [Verb("check_fileversion", HelpText = VERSION_EXPL)]
        class FileCheck_Options
        {
            [Option('f', "files", Required = true, HelpText = INPUT_FILE_MESSAGE)]
            public IEnumerable<String> InputFiles { get; set; }
        }

        [Verb("fctohex", HelpText = "Import an FC file to valid XML")]
        class FcImportOptions
        {
            [Option('f', "files", Required = true, HelpText = INPUT_FILE_MESSAGE)]
            public IEnumerable<String> InputFiles { get; set; }

            //optional interpreter file. If provided, the program will directly interpret the validated file. 
            [Option('i', "interpreter", Required = false, HelpText = INTERPRETER_FILE_HELP)]
            public String Interpreter { get; set; }
        }

        [Verb("hextofc", HelpText = "Reverse operation of fctohex.")]
        class FcExportOptions
        {
            [Option('f', "files", Required = true, HelpText = INPUT_FILE_MESSAGE)]
            public IEnumerable<String> InputFiles { get; set; }

            //optional interpreter file. If provided, the program will directly interpret the validated file. 
            [Option('i', "interpreter", Required = false, HelpText = "Interpreter file")]
            public String Interpreter { get; set; }
        }

        [Obsolete("decompress_Interpret has been merged with the verb decompress")]
        [Verb("decompress_interpret", HelpText = "DEPRACATED. decompress a filedb file and interpret it. An interpreter file is needed. ")]
        class Decompress_Interpret_Options
        {
            [Option('f', "files", Required = true, HelpText = "input files")]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('i', "interpreter", Required = true, HelpText = "Interpreter file")]
            public String Interpreter { get; set; }

            [Option('c', "CompressionVersion", Required = true, HelpText = "File Version: \n1 for Anno 1800 files up to GU12 \n2for Anno 1800 files after GU12")]
            public int CompressionVersion { get; set; }
        }
        [Obsolete("recompress_export has been merged with the verb recompress")]
        [Verb("recompress_export", HelpText = "DEPRACATED. reimport an xml file to filedb. An interpreter file is needed")]
        class Recompress_Export_Options
        {
            [Option('f', "files", Required = true, HelpText = "input files")]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('i', "interpreter", Required = true, HelpText = "Interpreter file")]
            public String Interpreter { get; set; }

            [Option('o', "outputFileExtension", Required = false, HelpText = "file Format of the output file")]
            public String OutputFileExtension { get; set; }

            [Option('c', "CompressionVersion", Required = true, HelpText = "File Version: \n1 for Anno 1800 files up to GU12 \n2for Anno 1800 files after GU12")]
            public int CompressionVersion { get; set; }
        }

        #endregion

        #region MainMethod
        private static void Main(string[] args) {

            var Functions = new FileDBCompressorFunctions();

            //todo make this pretty by adjusting writing to reading flow.
            CommandLine.Parser.Default.ParseArguments
                <
                    DecompressOptions, 
                    CompressOptions, 
                    InterpretOptions, 
                    toHexOptions, 
                    FileCheck_Options,
                    FcImportOptions, 
                    FcExportOptions, 
                    Decompress_Interpret_Options, 
                    Recompress_Export_Options
                > 
                (args).MapResult(

                //OPTIONS FOR DECOMPRESSING
                (DecompressOptions o) =>
                {
                    return Functions.Decompress(o.InputFiles, o.Interpreter, o.overwrite);
                },
                //OPTIONS FOR RECOMPRESSING
                (CompressOptions o) =>
                {
                    return Functions.Compress(o.InputFiles, o.Interpreter, o.OutputFileExtension, o.CompressionVersion, o.overwrite);
                },
                //OPTIONS FOR INTERPRETATION ONLY
                (InterpretOptions o) =>
                {
                    return Functions.Interpret(o.InputFiles, o.Interpreter, o.overwrite);
                },
                //OPTIONS FOR REINTERPRETATION ONLY
                (toHexOptions o) =>
                {
                    return Functions.Reinterpret(o.InputFiles, o.Interpreter, o.overwrite);
                },
                //CHECK COMPRESSION VERSION
                (FileCheck_Options o) =>
                {
                    return Functions.CheckFileVersion(o.InputFiles);
                },
                //OPTIONS FOR FC FILE IMPORT
                (FcImportOptions o) =>
                {
                    return Functions.FcFileImport(o.InputFiles, o.Interpreter);
                },
                //OPTIONS FOR FC FILE EXPORT
                (FcExportOptions o) =>
                {
                    return Functions.FcFileExport(o.InputFiles, o.Interpreter);
                },


                //---------DEPRACATED FUNCTIONS---------//
                (Decompress_Interpret_Options o ) =>
                {
                    return Functions.Decompress(o.InputFiles, o.Interpreter, false);
                }, 
                (Recompress_Export_Options o) => 
                {
                    return Functions.Compress(o.InputFiles, o.Interpreter, o.OutputFileExtension, o.CompressionVersion, false); 
                },
                e => 1
            ) ;
        }
        #endregion Methods
    }
}