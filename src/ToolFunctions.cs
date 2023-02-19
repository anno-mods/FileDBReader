using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using FileDBSerializing;
using FileDBReader.src.XmlRepresentation; 

namespace FileDBReader.src
{
    public class ToolFunctions
    {
        //file formats and names
        private static readonly String DefaultFileFormat = "filedb";
        private static readonly String DefaultFcFileFormat = "fc";
        private static readonly String InterpretedFileSuffix = "_interpreted";
        private static readonly String ReinterpretedFileSuffix = "_exported";

        //error message
        private static readonly String IOErrorMessage = "File Path wrong, File in use or does not exist.";

        //tools
        private Reader reader;
        private XmlExporter exporter;
        private Writer writer;
        private XmlInterpreter interpreter;
        private FcFileHelper FcFileHelper;


        public ToolFunctions()
        {
            reader = new Reader();
            exporter = new XmlExporter();
            writer = new Writer();
            interpreter = new XmlInterpreter();
            FcFileHelper = new FcFileHelper();
        }

        #region Decompress

        public int Decompress(IEnumerable<String>
            InputFiles,
            String InterpreterPath,
            bool overwrite,
            IEnumerable<String> ReplaceOps)
        {
            int returncode = 0;
            InvalidTagNameHelper.BuildAndAddReplaceOps(ReplaceOps);

            //Preload Interpreter
            Interpreter? Interpr = !String.IsNullOrEmpty(InterpreterPath) ?
                    new Interpreter(Interpreter.ToInterpreterDoc(SecureIoHandler.ReadHandle(InterpreterPath))) :
                    null;

            foreach (String s in InputFiles)
            {
                int return_c = Decompress(s, Interpr, overwrite);
                if (return_c == 1)
                    returncode = return_c;
            }
            return returncode;
        }

        private int Decompress(String InputFile,
            Interpreter? Interpr,
            bool overwrite)
        {
            try
            {
                using (Stream fs = SecureIoHandler.ReadHandle(InputFile))
                using (Stream output = SecureIoHandler.WriteHandle(Path.ChangeExtension(InputFile, "xml"), overwrite))
                {
                    XmlDocument result;
                    result = reader.Read(fs);
                    if (Interpr is not null)
                    {
                        Console.WriteLine($"Started interpreting {InputFile}");
                        result = interpreter.Interpret(result, Interpr);
                    }
                    result.Save(output);
                }
            }
            catch (IOException) {
                return -1;
            }
            catch (InvalidFileDBException exception)
            {
                Console.WriteLine($"File {InputFile} is not a valid FileDB Document: {exception.Message}");
                return -1;
            }
            catch (Exception other)
            {
                Console.WriteLine($"Terminated conversion of {InputFile} because of an unknown Error.");
                return -1;
            }
            return 0;
        }
        #endregion

        #region Compress

        public int Compress(IEnumerable<String> InputFiles,
            String InterpreterPath,
            String OutputFileExtension,
            int CompressionVersion,
            bool overwrite,
            IEnumerable<String> ReplaceOps)
        {
            int returncode = 0;
            //set output file extension
            var ext = OutputFileExtension ?? DefaultFileFormat;

            //Preload Interpreter
            Interpreter? Interpr = !String.IsNullOrEmpty(InterpreterPath) ?
                    new Interpreter(Interpreter.ToInterpreterDoc(SecureIoHandler.ReadHandle(InterpreterPath))) :
                    null;
            InvalidTagNameHelper.BuildAndAddReplaceOps(ReplaceOps);

            //convert all input files
            foreach (String s in InputFiles)
            {
                int return_c = Compress(s, Interpr, ext, CompressionVersion, overwrite);
                if (return_c != 0)
                    returncode = return_c;
            }
            return returncode;
        }

        private int Compress(String InputFile,
            Interpreter? interpreter,
            String OutputFileExtension,
            int CompressionVersion,
            bool overwrite)
        {
            try
            {
                XmlDocument result = new XmlDocument();
                using (Stream fs = SecureIoHandler.ReadHandle(InputFile))
                using (Stream output = SecureIoHandler.WriteHandle(Path.ChangeExtension(InputFile, OutputFileExtension), overwrite))
                {
                    result.Load(fs);

                    if (interpreter is not null)
                    {
                        Console.WriteLine($"Started reinterpreting {InputFile}");
                        result = exporter.Export(result, interpreter);
                    }
                    writer.Write(result, output, CompressionVersion);
                }
            }
            catch (IOException)
            {
                return -1;
            }
            catch (Exception other)
            {
                Console.WriteLine($"An Unknown Exception occured: {other.Message}");
                return -1;
            }
            return 0;
        }

        #endregion

        #region Interpret
        public int Interpret(
            IEnumerable<String> InputFiles,
            String InterpreterPath,
            bool overwrite,
            IEnumerable<String> ReplaceOps)
        {
            int returncode = 0;

            //Preload Interpreter
            Interpreter? Interpr = !String.IsNullOrEmpty(InterpreterPath) ?
                    new Interpreter(Interpreter.ToInterpreterDoc(SecureIoHandler.ReadHandle(InterpreterPath))) :
                    null;
            InvalidTagNameHelper.BuildAndAddReplaceOps(ReplaceOps);

            foreach (String s in InputFiles)
            {
                var return_c = Interpret(s, Interpr, overwrite);
                if (return_c != 0)
                    returncode = return_c;
            }
            return returncode;
        }

        private int Interpret(String InputFile,
            Interpreter? Interpr,
            bool overwrite)
        {
            try
            {
                var baseDoc = new XmlDocument();
                String FileNameNew = Path.GetFileNameWithoutExtension(InputFile) + InterpretedFileSuffix + ".xml";

                using (Stream input = SecureIoHandler.ReadHandle(InputFile))
                using (Stream output = SecureIoHandler.WriteHandle(FileNameNew, overwrite))
                {
                    baseDoc.Load(InputFile);
                    baseDoc = interpreter.Interpret(baseDoc, Interpr);
                    baseDoc.Save(output);
                }
            }
            catch (IOException)
            {
                return -1;
            }
            return 0;
        }
        #endregion

        #region Reinterpret 

        public int Reinterpret(IEnumerable<String> InputFiles,
            String InterpreterPath,
            bool overwrite,
            IEnumerable<String> ReplaceOps)
        {
            int returncode = 0;

            //Preload Interpreter
            Interpreter? Interpr = !String.IsNullOrEmpty(InterpreterPath) ?
                    new Interpreter(Interpreter.ToInterpreterDoc(SecureIoHandler.ReadHandle(InterpreterPath))) :
                    null;
            InvalidTagNameHelper.BuildAndAddReplaceOps(ReplaceOps);

            foreach (String s in InputFiles)
            {
                var return_c = Reinterpret(s, Interpr, overwrite);
                if (return_c != 0)
                    returncode = return_c;
            }
            return returncode;
        }

        private int Reinterpret(String InputFile,
            Interpreter Interpr,
            bool overwrite)
        {
            var inputDoc = new XmlDocument();
            try
            {
                String FileNameNew = Path.GetFileNameWithoutExtension(InputFile) + ReinterpretedFileSuffix + ".xml";
                using (Stream input = SecureIoHandler.WriteHandle(FileNameNew, overwrite))
                using (Stream output = SecureIoHandler.ReadHandle(InputFile))
                {
                    inputDoc.Load(input);
                    var doc = exporter.Export(inputDoc, Interpr);
                    {
                        doc.Save(output);
                    }
                }
            }
            catch (IOException)
            {
                return -1;
            }
            return 0;
        }

        #endregion

        #region CheckFileVersion

        public int CheckFileVersion(IEnumerable<String> InputFiles)
        {
            int returncode = 0;
            foreach (String s in InputFiles)
            {
                try
                {
                    using (Stream fs = SecureIoHandler.ReadHandle(s))
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

        #endregion

        #region FCFileImport
        public int FcFileImport(IEnumerable<String> InputFiles, String InterpreterPath, bool overwrite)
        {
            int returncode = 0;

            //Preload Interpreter
            Interpreter? Interpr = !String.IsNullOrEmpty(InterpreterPath) ?
                    new Interpreter(Interpreter.ToInterpreterDoc(SecureIoHandler.ReadHandle(InterpreterPath))) :
                    null;

            foreach (String s in InputFiles)
            {
                FcFileImport(s, Interpr, overwrite);
            }
            return returncode;
        }

        private void FcFileImport(String InputFile, Interpreter? Interpr, bool overwrite)
        {
            try
            {
                String FileNameNew = Path.GetFileNameWithoutExtension(InputFile) + ".xml";

                using (Stream fs = SecureIoHandler.WriteHandle(FileNameNew, overwrite))
                using (Stream input = SecureIoHandler.ReadHandle(InputFile))
                {
                    var result = FcFileHelper.ReadFcFile(input);
                    if (Interpr is not null)
                    {
                        result = interpreter.Interpret(result, Interpr);
                    }
                    result.Save(fs);
                }
            }
            catch (IOException ex)
            {

            }
        }
        #endregion

        #region FcExport
        public int FcFileExport(IEnumerable<String> InputFiles, String InterpreterPath, bool overwrite, String OutputFileExtension)
        {
            int returncode = 0;

            var ext = OutputFileExtension ?? DefaultFcFileFormat;

            //Preload Interpreter
            Interpreter? Interpr = !String.IsNullOrEmpty(InterpreterPath) ?
                    new Interpreter(Interpreter.ToInterpreterDoc(SecureIoHandler.ReadHandle(InterpreterPath))) :
                    null;

            foreach (String s in InputFiles)
            {
                FcFileExport(s, Interpr, overwrite, ext);
            }
            return returncode;
        }

        private void FcFileExport(String InputFile, Interpreter? Interpr, bool overwrite, String OutputFileExtension)
        {
            try
            {
                var path = Path.ChangeExtension(Path.GetFileNameWithoutExtension(InputFile), OutputFileExtension);

                using (Stream input = SecureIoHandler.ReadHandle(InputFile))
                using (var output = SecureIoHandler.WriteHandle(path, overwrite))
                {
                    XmlDocument exported = new XmlDocument();
                    exported.Load(input);
                    if (Interpr is not null)
                    {
                        exported = exporter.Export(exported, Interpr);
                    }
                    FcFileHelper.ConvertFile(FcFileHelper.XmlFileToStream(exported), ConversionMode.Write, output);
                }
            }
            catch (IOException)
            {
               
            }
        }
        #endregion
    }
}
