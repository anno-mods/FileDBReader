using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace FileDBReader.src
{
    public class FileDBCompressorFunctions
    {
        //file formats and names
        private static readonly String DefaultFileFormat = "filedb";
        private static readonly String InterpretedFileSuffix = "_interpreted";
        private static readonly String ReinterpretedFileSuffix = "_exported";
        private static readonly String FcImportedFileSuffix = "_fcimport";
        private static readonly String FcExportedFileSuffix = "_fcexport";

        //error message
        private static readonly String IOErrorMessage = "File Path wrong, File in use or does not exist.";

        //tools
        private FileReader reader;
        private XmlExporter exporter;
        private FileWriter writer;
        private XmlInterpreter interpreter;
        private FcFileHelper FcFileHelper; 

        public FileDBCompressorFunctions()
        {
            reader = new FileReader();
            exporter = new XmlExporter();
            writer = new FileWriter();
            interpreter = new XmlInterpreter();
            FcFileHelper = new FcFileHelper();
        }

        public int Decompress(IEnumerable<String> InputFiles, String Interpreter)
        {
            int returncode = 0;
            var interpreterDoc = new XmlDocument();
            interpreterDoc.Load(Interpreter);
            foreach (String s in InputFiles)
            {
                Task.Run(() => {
                    var result = reader.ReadFile(s);
                    if (Interpreter != null)
                    {
                        try
                        {
                            result = interpreter.Interpret(result, interpreterDoc);
                            result.Save(Path.ChangeExtension(s, "xml"));
                        }
                        catch (IOException ex)
                        {
                            Console.WriteLine(IOErrorMessage + "\n {0}", ex.Message);
                            returncode = -1;
                        }
                    }
                } );
            }
            return returncode;
        }

        public int Compress(IEnumerable<String> InputFiles, String Interpreter, String OutputFileExtension, int CompressionVersion)
        {
            int returncode = 0; 
            //set output file extension
            var ext = "";
            if (OutputFileExtension != null)
                ext = OutputFileExtension;
            else
                ext = DefaultFileFormat;

            //convert all input files
            foreach (String s in InputFiles)
            {
                Task.Run( () => {
                    if (Interpreter != null)
                    {
                        try
                        {
                            var result = exporter.Export(s, Interpreter);
                            writer.Export(result, ext, s, CompressionVersion);
                        }
                        catch (IOException ex)
                        {
                            Console.WriteLine(IOErrorMessage + "\n {0}", ex.Message);
                            returncode = -1;
                        }
                    }
                    else
                    {
                        writer.Export(s, ext, CompressionVersion);
                    }
                } );
            }
            return returncode;
        }

        public int Interpret(IEnumerable<String> InputFiles, String Interpreter)
        {
            int returncode = 0; 
            foreach (String s in InputFiles)
            {
                Task.Run( () => {
                    try
                    {
                        var doc = interpreter.Interpret(s, Interpreter);
                        doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(s, InterpretedFileSuffix), "xml"));
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(IOErrorMessage + "\n {0}", ex.Message);
                        returncode = -1;
                    }
                } );
            }
            return returncode;
        }

        public int Reinterpret(IEnumerable<String> InputFiles, String Interpreter)
        {
            int returncode = 0; 
            foreach (String s in InputFiles)
            {
                Task.Run(() =>
                {
                    try
                    {
                        var doc = exporter.Export(s, Interpreter);
                        doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(s, ReinterpretedFileSuffix), "xml"));
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(IOErrorMessage + "\n {0}", ex.Message);
                        returncode = -1;
                    }
                });
            }
            return returncode;
        }

        public int CheckFileVersion(IEnumerable<String> InputFiles)
        {
            int returncode = 0; 
            foreach (String s in InputFiles)
            {
                Task.Run(() => {
                    try
                    {
                        Console.WriteLine("{0} uses Compression Version {1}", s, reader.CheckFileVersion(s));
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(IOErrorMessage + "\n {0}", ex.Message);
                        returncode = -1;
                    }
                });
            }
            return returncode;
        }

        public int FcFileImport(IEnumerable<String> InputFiles, String Interpreter)
        {
            int returncode = 0; 
            foreach (String s in InputFiles)
            {
                Task.Run(() =>
                {
                    try
                    {
                        var result = FcFileHelper.ReadFcFile(s);
                        if (Interpreter != null)
                        {
                            try
                            {
                                var interpreterDoc = new XmlDocument();
                                interpreterDoc.Load(Interpreter);
                                result = interpreter.Interpret(result, interpreterDoc);
                            }
                            catch (IOException ex)
                            {
                                Console.WriteLine(IOErrorMessage + "\n {0}", ex.Message);
                                returncode = -1;
                            }
                        }
                        result.Save(Path.ChangeExtension(HexHelper.AddSuffix(s, FcImportedFileSuffix), "xml"));
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(IOErrorMessage + "\n {0}", ex.Message);
                        returncode = -1;
                    }
                });
            }
            return returncode; 
        }

        public int FcFileExport(IEnumerable<String> InputFiles, String Interpreter)
        {
            int returncode = 0; 
            foreach (String s in InputFiles)
            {
                Task.Run( () => {
                    try
                    {
                        XmlDocument exported;
                        if (Interpreter != null)
                        {
                            exported = exporter.Export(s, Interpreter);
                        }
                        else
                        {
                            exported = new XmlDocument();
                            exported.Load(s);
                        }

                        var Written = FcFileHelper.ConvertFile(FcFileHelper.XmlFileToStream(exported), ConversionMode.Write);
                        FcFileHelper.SaveStreamToFile(Written, Path.ChangeExtension(HexHelper.AddSuffix(s, FcExportedFileSuffix), "xml"));
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(IOErrorMessage + "\n {0}", ex.Message);
                        returncode = -1;
                    }
                });
            }

            return returncode;
        }
    }
}
