using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CommandLine;
using FileDBReader.src;

namespace FileDBReader {

  internal class Program {

        #region CmdLineOptions
        [Verb("decompress", HelpText = "Decompress a file from filedb to xml. Data will be represented as hex strings.")]
        class DecompressOptions
        {
            [Option('f', "file", Required = true, HelpText = "Required. Input files to be decompressed.")]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('c', "CompressionVersion", Required = false, HelpText = "Optional. File Version: \n1 for Anno 1800 files up to GU12 \n2for Anno 1800 files after GU12")]
            public int CompressionVersion { get; set; }

            //optional interpreter file. If provided, it will directly interpret the decompressed file. 
            [Option('i', "interpreter", Required = false, HelpText = "Optional. Interpreter file, if provided, the program will directly interpret the decompressed file and print the result.")]
            public String Interpreter { get; set; }
        }

        [Verb("compress", HelpText = "Recompress an xml file to filedb. Requires data to be represented as hex strings")]
        class CompressOptions
        {
            [Option('f', "file", Required = true, HelpText = "Required. Input files to be compressed")]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('o', "outputFileExtension", Required = false, HelpText = "Optional. File Format of the output file")]
            public String OutputFileExtension{ get; set; }

            [Option('c', "CompressionVersion", Required = true, HelpText = "Required. File Version: \n1 for Anno 1800 files up to GU12 \n2for Anno 1800 files after GU12")]
            public int CompressionVersion { get; set; }

            //optional interpreter file. If provided, it will go directly from an interpreted file to final result
            [Option('i', "interpreter", Required = true, HelpText = "Optional. Interpreter file, if provided, the program will directly compress an interpreted file and print the result.")]
            public String Interpreter { get; set; }
        }

        [Verb("interpret", HelpText = "Interpret an xml file that uses hex strings as texts. An interpreter file is needed")]
        class InterpretOptions
        {
            [Option('f', "file", Required = true, HelpText = "Required. Input files")]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('i', "interpreter", Required = true, HelpText = "Required. Interpreter file")]
            public String Interpreter { get; set; }
        }

        [Verb("toHex", HelpText = "Convert all text in an xml file to hex. An interpreter file is needed")]
        class toHexOptions
        {
            [Option('f', "file", Required = true, HelpText = "Required. Input files")]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('i', "interpreter", Required = true, HelpText = "Required. Interpreter file")]
            public String Interpreter { get; set; }
        }

        [Verb("check_fileversion", HelpText = "Check the compression version of a file.")]
        class FileCheck_Options
        {
            [Option('f', "file", Required = true, HelpText = "Required. Input files")]
            public IEnumerable<String> InputFiles { get; set; }
        }

        [Verb("fctohex", HelpText = "Import an FC file to valid XML")]
        class FcImportOptions
        {
            [Option('f', "file", Required = true, HelpText = "Required. Input files")]
            public IEnumerable<String> InputFiles { get; set; }

            //optional interpreter file. If provided, the program will directly interpret the validated file. 
            [Option('i', "interpreter", Required = false, HelpText = "Optional. Interpreter file")]
            public String Interpreter { get; set; }
        }

        [Verb("hextofc", HelpText = "Reverse operation of fctohex.")]
        class FcExportOptions
        {
            [Option('f', "file", Required = true, HelpText = "input files")]
            public IEnumerable<String> InputFiles { get; set; }

            //optional interpreter file. If provided, the program will directly interpret the validated file. 
            [Option('i', "interpreter", Required = false, HelpText = "Interpreter file")]
            public String Interpreter { get; set; }
        }

        //Functions starting from here are depracated
        [Obsolete("decompress_Interpret has been merged with the verb decompress")]
        [Verb("decompress_interpret", HelpText = "DEPRACATED. decompress a filedb file and interpret it. An interpreter file is needed. ")]
        class Decompress_Interpret_Options
        {
            [Option('f', "file", Required = true, HelpText = "input files")]
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
            [Option('f', "file", Required = true, HelpText = "input files")]
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

            //todo (taubenangriff) delete this when removing depracated functions
            var reader = new FileReader();
            var exporter = new XmlExporter();
            var writer = new FileWriter();
            var interpreter = new XmlInterpreter();
            var FcFileHelper = new FcFileHelper();

            var Functions = new FileDBCompressorFunctions();

            //todo make this pretty by adjusting writing to reading flow.
            CommandLine.Parser.Default.ParseArguments
                <
                    DecompressOptions, 
                    CompressOptions, 
                    InterpretOptions, 
                    toHexOptions, 
                    FcImportOptions, 
                    FcExportOptions, 
                    Decompress_Interpret_Options, 
                    Recompress_Export_Options
                > 
                (args).MapResult(

                //OPTIONS FOR DECOMPRESSING
                (DecompressOptions o) =>
                {
                    return Functions.Decompress(o.InputFiles, o.Interpreter);
                },
                //OPTIONS FOR RECOMPRESSING
                (CompressOptions o) =>
                {
                    return Functions.Compress(o.InputFiles, o.Interpreter, o.OutputFileExtension, o.CompressionVersion);
                },
                //OPTIONS FOR INTERPRETATION ONLY
                (InterpretOptions o) =>
                {
                    return Functions.Interpret(o.InputFiles, o.Interpreter);
                },
                //OPTIONS FOR REINTERPRETATION ONLY
                (toHexOptions o) =>
                {
                    return Functions.Reinterpret(o.InputFiles, o.Interpreter);
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

                //DEPRACATED ARGUMENTS START HERE
                //I don't want to rewrite this too :) will be removed anyway
                (Decompress_Interpret_Options o) =>
                {
                    foreach (String s in o.InputFiles)
                    {
                        var interpreterDoc = new XmlDocument();
                        interpreterDoc.Load(o.Interpreter);
                        var doc = interpreter.Interpret(reader.ReadFile(s), interpreterDoc);
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
                        try
                        {
                            writer.Export(exporter.Export(s, o.Interpreter), o.OutputFileExtension, s, o.CompressionVersion);
                        }
                        catch (IOException e)
                        {
                            Console.WriteLine("File Path wrong, File in use or does not exist.");
                        }
                    }
                    return 0;
                },
                e => 1
            ) ;
        }
        #endregion Methods
    }
}