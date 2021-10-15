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
            [Option('c', "CompressionVersion", Required = true, HelpText = "File Version: \n1 for Anno 1800 files up to GU12 \n2for Anno 1800 files after GU12")]
            public int CompressionVersion { get; set; }
        }

        [Verb("compress", HelpText = "Recompress an xml file to filedb. Requires data to be represented as hex strings")]
        class CompressOptions
        {
            [Option('f', "file", Required = true, HelpText = "input files")]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('o', "outputFileExtension", Required = false, HelpText = "file Format of the output file")]
            public String OutputFileExtension{ get; set; }
            [Option('c', "CompressionVersion", Required = true, HelpText = "File Version: \n1 for Anno 1800 files up to GU12 \n2for Anno 1800 files after GU12")]
            public int CompressionVersion { get; set; }
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

            [Option('c', "CompressionVersion", Required = true, HelpText = "File Version: \n1 for Anno 1800 files up to GU12 \n2for Anno 1800 files after GU12")]
            public int CompressionVersion { get; set; }
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

            [Option('c', "CompressionVersion", Required = true, HelpText = "File Version: \n1 for Anno 1800 files up to GU12 \n2for Anno 1800 files after GU12")]
            public int CompressionVersion { get; set; }
        }
        [Verb("check_fileversion", HelpText = "Check the compression version of a file.")]
        class FileCheck_Options
        {
            [Option('f', "file", Required = true, HelpText = "input files")]
            public IEnumerable<String> InputFiles { get; set; }
        }
        [Verb("fctohex", HelpText = "Import an FC file to valid XML")]
        class FcImportOptions
        {
            [Option('f', "file", Required = true, HelpText = "input files")]
            public IEnumerable<String> InputFiles { get; set; }
        }
        [Verb("hextofc", HelpText = "Reverse operation of fctohex.")]
        class FcExportOptions
        {
            [Option('f', "file", Required = true, HelpText = "input files")]
            public IEnumerable<String> InputFiles { get; set; }
        }
        [Verb("fctohex_interpret", HelpText = "Import an FC file to valid XML and interpret it using an interpreter.")]
        class FcImport_InterpretOptions
        {
            [Option('f', "file", Required = true, HelpText = "input files")]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('i', "interpreter", Required = true, HelpText = "Interpreter file")]
            public String Interpreter { get; set; }
        }
        [Verb("hextofc_export", HelpText = "Exports the xml version back to an fc file using an interpreter.")]
        class FcExport_ExportOptions
        {
            [Option('f', "file", Required = true, HelpText = "input files")]
            public IEnumerable<String> InputFiles { get; set; }

            [Option('i', "interpreter", Required = true, HelpText = "Interpreter file")]
            public String Interpreter { get; set; }

            [Option('o', "outputFileExtension", Required = false, HelpText = "file Format of the output file")]
            public String OutputFileExtension { get; set; }
        }
        #endregion

        #region MainMethod
        private static void Main(string[] args) {

            var reader = new FileReader();
            var exporter = new XmlExporter();
            var writer = new FileWriter();
            var interpreter = new XmlInterpreter();
            var FcFileHelper = new FcFileHelper(); 


            //todo make this pretty by adjusting writing to reading flow.
            CommandLine.Parser.Default.ParseArguments<DecompressOptions, CompressOptions, InterpretOptions, toHexOptions, Decompress_Interpret_Options, Recompress_Export_Options, FcImportOptions, FcImport_InterpretOptions, FcExportOptions, FcExport_ExportOptions>(args).MapResult(
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
                            writer.Export(s, ext, o.CompressionVersion);
                        return 0;
                    },
                    (InterpretOptions o) =>
                    {
                        foreach (String s in o.InputFiles) 
                        {
                            try
                            {
                                var doc = interpreter.Interpret(s, o.Interpreter);
                                doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(s, "_interpreted"), "xml"));
                            }
                            catch (IOException e) {
                                Console.WriteLine("File Path wrong, File in use or does not exist.");
                            }
                        }
                        return 0;
                    },
                    (toHexOptions o) =>
                    {
                        foreach (String s in o.InputFiles) {
                            try
                            {
                                var doc = exporter.Export(s, o.Interpreter);
                                doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(s, "_exported"), "xml"));
                            }
                            catch (IOException e)
                            {
                                Console.WriteLine("File Path wrong, File in use or does not exist.");
                            }
                        }
                        return 0; 
                    },
                    (Decompress_Interpret_Options o) =>
                    {
                        foreach (String s in o.InputFiles) {
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
                    (FileCheck_Options o) =>
                    {
                        foreach (String s in o.InputFiles)
                        {
                            Console.WriteLine("{0} uses Compression Version {1}",s, reader.CheckFileVersion(s));
                        }

                        return 0;
                     }
                    ,
                    (FcImportOptions o) =>
                    {
                        foreach (String s in o.InputFiles)
                        {
                            try
                            {
                                var doc = FcFileHelper.ReadFcFile(s);
                                doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(s, "_fcimport"), "xml"));
                            }
                            catch (IOException e)
                            {
                                Console.WriteLine("File Path wrong, File in use or does not exist.");
                            }
                        }

                        return 0; 
                    }, 
                    (FcExportOptions o) =>
                    {
                        foreach(String s in o.InputFiles)
                        {
                            try
                            {
                                var doc = FcFileHelper.ReadFcFile(s);
                                doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(s, "_fcexport"), "xml"));
                            }
                            catch (IOException e)
                            {
                                Console.WriteLine("File Path wrong, File in use or does not exist.");
                            }
                        }

                        return 0;
                    },
                    (FcImport_InterpretOptions o) =>
                    {
                        foreach (String s in o.InputFiles)
                        {
                            try
                            {
                                var interpreterDoc = new XmlDocument();
                                interpreterDoc.Load(o.Interpreter);
                                var doc = interpreter.Interpret(FcFileHelper.ReadFcFile(s), interpreterDoc);
                                doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(s, "_fc_i"), "xml"));
                            }
                            catch (IOException e)
                            {
                                Console.WriteLine("File Path wrong, File in use or does not exist.");
                            }
                        }

                        return 0; 
                    },
                    (FcExport_ExportOptions o) =>
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
                                var exported = exporter.Export(s, o.Interpreter);
                                var Written = FcFileHelper.ConvertFile(FcFileHelper.XmlFileToStream(exported), ConversionMode.Write);
                                FcFileHelper.SaveStreamToFile(Written, Path.ChangeExtension(HexHelper.AddSuffix(s, "_fc_e"), "xml"));
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