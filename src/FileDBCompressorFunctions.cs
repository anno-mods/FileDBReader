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
        private Reader reader;
        private XmlExporter exporter;
        private Writer writer;
        private XmlInterpreter interpreter;
        private FcFileHelper FcFileHelper;

        //use new serializer options for better performance in the future
        private FileDBDeserializer<FileDBDocument_V2> FileDBDeserializer;
        private FileDBSerializer FileDBSerializer;
        private FileDbXmlSerializer XmlSerializer; 


        public FileDBCompressorFunctions()
        {
            reader = new Reader();
            exporter = new XmlExporter();

            writer = new Writer();
            interpreter = new XmlInterpreter();
            FcFileHelper = new FcFileHelper();

            FileDBDeserializer = new FileDBDeserializer<FileDBDocument_V2>();
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
                try
                {
                    using (FileStream fs = SecureIoHandler.ReadHandle(s))
                    {
                        XmlDocument result;
                        result = reader.Read(fs);
                        if (InterpreterPath != null)
                        {
                            result = interpreter.Interpret(result, Interpr);
                        }
                        using (FileStream output = SecureIoHandler.ReadHandle(Path.ChangeExtension(s, "xml")))
                        {
                            result.Save(output);
                        }
                    }
                }
                catch (IOException)
                {
                    returncode = -1;
                }
                
            }
            return returncode;
        }

        public int Compress(IEnumerable<String> InputFiles, String InterpreterPath, String OutputFileExtension, int CompressionVersion, bool overwrite)
        {
            int returncode = 0;
            //set output file extension
            var ext = OutputFileExtension ?? DefaultFileFormat;

            //Preload Interpreter
            Interpreter Interpr = null;
            if (InterpreterPath != null)
            {
                Interpr = new Interpreter(Interpreter.ToInterpreterDoc(InterpreterPath));
            }

            //convert all input files
            foreach (String s in InputFiles)
            {
                try
                {
                    XmlDocument result = new XmlDocument();
                    using (FileStream fs = SecureIoHandler.ReadHandle(s))
                    {
                        result.Load(fs);
                    }
                    if (InterpreterPath != null)
                    {
                        result = exporter.Export(result, Interpr);
                    }
                    using (FileStream fs = SecureIoHandler.WriteHandle(s, overwrite))
                    {
                        writer.Write(result, fs, CompressionVersion);
                    }
                }
                catch (IOException)
                {
                    returncode = -1;
                }
            }
            return returncode;
        }
        public int Interpret(IEnumerable<String> InputFiles, String InterpreterPath, bool overwrite)
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
                    using (FileStream input = SecureIoHandler.ReadHandle(s))
                    {
                        baseDoc.Load(s);
                    }
                    baseDoc = interpreter.Interpret(baseDoc, Interpr);
                    //Save
                    String FileNameNew = Path.GetFileNameWithoutExtension(s) + InterpretedFileSuffix + ".xml";
                    using (FileStream output = SecureIoHandler.WriteHandle(FileNameNew, overwrite))
                    {
                        baseDoc.Save(output);
                    }
                }
                catch (IOException)
                {
                    returncode = -1;
                }
            }
            return returncode;
        }

        public int Reinterpret(IEnumerable<String> InputFiles, String InterpreterPath, bool overwrite)
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
                var inputDoc = new XmlDocument();
                try
                {
                    using (FileStream fs = SecureIoHandler.ReadHandle(s))
                    {
                        inputDoc.Load(fs);
                    }
                    var doc = exporter.Export(inputDoc, Interpr);

                    String FileNameNew = Path.GetFileNameWithoutExtension(s) + ReinterpretedFileSuffix + ".xml";
                    using (FileStream fs = SecureIoHandler.WriteHandle(FileNameNew, overwrite))
                    {
                        doc.Save(fs);
                    }
                }
                catch (IOException)
                {
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
                    using (FileStream fs = SecureIoHandler.ReadHandle(s))
                    {
                        Console.WriteLine("{0} uses Compression Version {1}", s, VersionDetector.GetCompressionVersion(fs));
                    }
                }
                catch (IOException)
                {
                    returncode = -1;
                }
            }
            return returncode;
        }


        //TODO IMPLEMENT SAFE FILE HANDLES
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
                catch (IOException)
                {
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
                catch (IOException)
                {
                    returncode = -1;
                }
            }

            return returncode;
        }
    }
}
