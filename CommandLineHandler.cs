using System;
using System.Collections.Generic;
using CommandLine;
using FileDBReader.src;

namespace FileDBReader {

  internal class CommandLineHandler {

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

            [Option('z', "replace_Names", Required = false, HelpText = "String Tuples of names that should be replaced")]
            public IEnumerable<String> ReplaceOperations { get; set; }
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

            [Option('z', "replace_Names", Required = false, HelpText = "String Tuples of names that should be replaced")]
            public IEnumerable<String> ReplaceOperations { get; set; }
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

            [Option('z', "replace_Names", Required = false, HelpText = "String Tuples of names that should be replaced")]
            public IEnumerable<String> ReplaceOperations { get; set; }

            [Option('o', "outputFileExtension", Required = false, HelpText = OUTPUT_FILEFORMAT_HELP)]
            public String OutputFileExtension { get; set; }

            [Option('d', "in_is_out_dir", Required = false, Default = false, HelpText = "If set, the output directory becomes the input directory. This being an option is purely for backwards compabilities sake.")]
            public bool InIsOut { get; set; }

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

            [Option('z', "replace_Names", Required = false, HelpText = "String Tuples of names that should be replaced")]
            public IEnumerable<String> ReplaceOperations { get; set; }

            [Option('d', "in_is_out_dir", Required = false, Default = false, HelpText = "If set, the output directory becomes the input directory. This being an option is purely for backwards compabilities sake.")]
            public bool InIsOut { get; set; }

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

            [Option('y', "overwrite", Required = false, Default = false)]
            public bool overwrite { get; set; }

            [Option('d', "in_is_out_dir", Required = false, Default = false, HelpText = "If set, the output directory becomes the input directory. This being an option is purely for backwards compabilities sake.")]
            public bool InIsOut { get; set; }
        }

        [Verb("hextofc", HelpText = "Reverse operation of fctohex.")]
        class FcExportOptions
        {
            [Option('f', "files", Required = true, HelpText = INPUT_FILE_MESSAGE)]
            public IEnumerable<String> InputFiles { get; set; }

            //optional interpreter file. If provided, the program will directly interpret the validated file. 
            [Option('i', "interpreter", Required = false, HelpText = "Interpreter file")]
            public String Interpreter { get; set; }

            [Option('y', "overwrite", Required = false, Default = false)]
            public bool overwrite { get; set; }

            [Option('o', "outputFileExtension", Required = false, HelpText = OUTPUT_FILEFORMAT_HELP)]
            public String OutputFileExtension { get; set; }

            [Option('d', "in_is_out_dir", Required = false, Default = false, HelpText = "If set, the output directory becomes the input directory. This being an option is purely for backwards compabilities sake.")]
            public bool InIsOut { get; set; }

        }
        #endregion

        #region MainMethod
        private static void Main(string[] args) {

            var Functions = new ToolFunctions();

            //todo make this pretty by adjusting writing to reading flow.
            CommandLine.Parser.Default.ParseArguments
                <
                    DecompressOptions, 
                    CompressOptions, 
                    InterpretOptions, 
                    toHexOptions, 
                    FileCheck_Options,
                    FcImportOptions, 
                    FcExportOptions
                > 
                (args).MapResult(

                //OPTIONS FOR DECOMPRESSING
                (DecompressOptions o) =>
                {
                    return Functions.Decompress(o.InputFiles, o.Interpreter, o.overwrite, o.ReplaceOperations);
                },
                //OPTIONS FOR RECOMPRESSING
                (CompressOptions o) =>
                {
                    return Functions.Compress(o.InputFiles, o.Interpreter, o.OutputFileExtension, o.CompressionVersion, o.overwrite, o.ReplaceOperations);
                },
                //OPTIONS FOR INTERPRETATION ONLY
                (InterpretOptions o) =>
                {
                    return Functions.Interpret(o.InputFiles, o.Interpreter, o.overwrite, o.ReplaceOperations, o.InIsOut);
                },
                //OPTIONS FOR REINTERPRETATION ONLY
                (toHexOptions o) =>
                {
                    return Functions.Reinterpret(o.InputFiles, o.Interpreter, o.overwrite, o.ReplaceOperations, o.InIsOut);
                },
                //CHECK COMPRESSION VERSION
                (FileCheck_Options o) =>
                {
                    return Functions.CheckFileVersion(o.InputFiles);
                },
                //OPTIONS FOR FC FILE IMPORT
                (FcImportOptions o) =>
                {
                    return Functions.FcFileImport(o.InputFiles, o.Interpreter, o.overwrite, o.InIsOut);
                },
                //OPTIONS FOR FC FILE EXPORT
                (FcExportOptions o) =>
                {
                    return Functions.FcFileExport(o.InputFiles, o.Interpreter, o.overwrite, o.OutputFileExtension, o.InIsOut);
                },
                e => 1
            ) ;
        }
        #endregion Methods
    }
}