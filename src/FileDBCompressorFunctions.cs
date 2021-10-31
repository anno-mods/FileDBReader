using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using FileDBSerializing;
using FileDBReader.src.XmlSerialization; 

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

        //use new serializer options for better performance in the future
        private FileDBDeserializer FileDBDeserializer;
        private FileDBSerializer FileDBSerializer;
        private FileDbXmlSerializer XmlSerializer; 


        public FileDBCompressorFunctions()
        {
            reader = new FileReader();
            exporter = new XmlExporter();

            writer = new FileWriter();
            interpreter = new XmlInterpreter();
            FcFileHelper = new FcFileHelper();

            FileDBDeserializer = new FileDBDeserializer();
            FileDBSerializer = new FileDBSerializer(); 
            XmlSerializer = new FileDbXmlSerializer();
        }

        public int Decompress(IEnumerable<String> InputFiles, String InterpreterPath)
        {
            int returncode = 0;

            //Preload Interpreter
            Interpreter Interpr = null; 
            if (InterpreterPath != null)
            {
                Interpr = new Interpreter(Interpreter.ToInterpreterDoc(InterpreterPath));
            }

            foreach (String s in InputFiles)
            {
                var result = reader.ReadFile(s);
                if (InterpreterPath != null)
                {
                    try
                    {
                        result = interpreter.Interpret(result, Interpr);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(IOErrorMessage + "\n {0}", ex.Message);
                        returncode = -1;
                    }
                }
                result.Save(Path.ChangeExtension(s, "xml"));
            }
            return returncode;
        }

        public int Compress(IEnumerable<String> InputFiles, String InterpreterPath, String OutputFileExtension, int CompressionVersion)
        {
            int returncode = 0; 
            //set output file extension
            var ext = "";
            if (OutputFileExtension != null)
                ext = OutputFileExtension;
            else
                ext = DefaultFileFormat;

            //Preload Interpreter
            Interpreter Interpr = null;
            if (InterpreterPath != null)
            {
                Interpr = new Interpreter(Interpreter.ToInterpreterDoc(InterpreterPath));
            }

            //convert all input files
            foreach (String s in InputFiles)
            {
                if (InterpreterPath != null)
                {
                    try
                    {
                        var doc = new XmlDocument();
                        doc.Load(s);
                        var result = exporter.Export(doc, Interpr);
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
            }
            return returncode;
        }
        public int Interpret(IEnumerable<String> InputFiles, String InterpreterPath)
        {
            int returncode = 0;

            //Preload Interpreter
            Interpreter Interpr = null;
            if (InterpreterPath != null)
            {
                Interpr = new Interpreter(Interpreter.ToInterpreterDoc(InterpreterPath));
            }

            foreach (String s in InputFiles)
            {
                try
                {
                    var baseDoc = new XmlDocument();
                    baseDoc.Load(s);
                    var doc = interpreter.Interpret(baseDoc, Interpr);
                    doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(s, InterpretedFileSuffix), "xml"));
                }
                catch (IOException ex)
                {
                    Console.WriteLine(IOErrorMessage + "\n {0}", ex.Message);
                    returncode = -1;
                }
            }
            return returncode;
        }

        public int Reinterpret(IEnumerable<String> InputFiles, String InterpreterPath)
        {
            int returncode = 0;

            //Preload Interpreter
            Interpreter Interpr = null;
            if (InterpreterPath != null)
            {
                Interpr = new Interpreter(Interpreter.ToInterpreterDoc(InterpreterPath));
            }

            foreach (String s in InputFiles)
            {
                try
                {
                    var inputDoc = new XmlDocument();
                    inputDoc.Load(s);
                    var doc = exporter.Export(inputDoc, Interpr);
                    doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(s, ReinterpretedFileSuffix), "xml"));
                }
                catch (IOException ex)
                {
                    Console.WriteLine(IOErrorMessage + "\n {0}", ex.Message);
                    returncode = -1;
                }
            }
            return returncode;
        }

        public int CheckFileVersion(IEnumerable<String> InputFiles)
        {
            int returncode = 0; 
            foreach (String s in InputFiles)
            {
                try
                {
                    Console.WriteLine("{0} uses Compression Version {1}", s, reader.CheckFileVersion(s));
                }
                catch (IOException ex)
                {
                    Console.WriteLine(IOErrorMessage + "\n {0}", ex.Message);
                    returncode = -1;
                }
            }
            return returncode;
        }

        public int FcFileImport(IEnumerable<String> InputFiles, String InterpreterPath)
        {
            int returncode = 0; 
            
            //Preload Interpreter
            Interpreter Interpr = null;
            if (InterpreterPath != null)
            {
                Interpr = new Interpreter(Interpreter.ToInterpreterDoc(InterpreterPath));
            }

            foreach (String s in InputFiles)
            {
                try
                {
                    var result = FcFileHelper.ReadFcFile(s);
                    if (InterpreterPath != null)
                    {
                        try
                        {
                            result = interpreter.Interpret(result, Interpr);
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
            }
            return returncode; 
        }

        public int FcFileExport(IEnumerable<String> InputFiles, String InterpreterPath)
        {
            int returncode = 0;
            //Preload Interpreter
            Interpreter Interpr = null;
            if (InterpreterPath != null)
            {
                Interpr = new Interpreter(Interpreter.ToInterpreterDoc(InterpreterPath));
            }

            foreach (String s in InputFiles)
            {
                try
                {
                    XmlDocument exported = new XmlDocument();
                    if (InterpreterPath != null)
                    {
                        exported.Load(s);
                        exported = exporter.Export(exported, Interpr);
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
            }

            return returncode;
        }
    }
}
