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
            writer = new Writer();
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
            Interpreter? Interpr = null;
            if (!String.IsNullOrEmpty(InterpreterPath))
            {
                using var interpreter_stream = SecureIoHandler.ReadHandleWithInterpreterRedirect(InterpreterPath);
                if (interpreter_stream is null)
                    return -1;
                Interpr = new Interpreter(Interpreter.ToInterpreterDoc(interpreter_stream));
            }

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
                using (var fs = SecureIoHandler.ReadHandle(InputFile))
                using (var output = SecureIoHandler.WriteHandle(Path.ChangeExtension(InputFile, "xml"), overwrite))
                {
                    if (fs is null || output is null)
                        return -1;

                    XmlDocument result;
                    result = reader.Read(fs);
                    if (Interpr is not null)
                    {
                        Console.WriteLine($"Started interpreting {InputFile}");
                        var interpreter = new XmlInterpreter(result, Interpr);
                        result = interpreter.Run();
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
            Interpreter? Interpr = null; 
            if (!String.IsNullOrEmpty(InterpreterPath))
            {
                using var interpreter_stream = SecureIoHandler.ReadHandleWithInterpreterRedirect(InterpreterPath);
                if (interpreter_stream is null)
                    return -1;
                Interpr = new Interpreter(Interpreter.ToInterpreterDoc(interpreter_stream));
            }
            
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
                using (var fs = SecureIoHandler.ReadHandle(InputFile))
                using (var output = SecureIoHandler.WriteHandle(Path.ChangeExtension(InputFile, OutputFileExtension), overwrite))
                {
                    if (fs is null || output is null)
                        return -1;

                    result.Load(fs);

                    if (interpreter is not null)
                    {
                        Console.WriteLine($"Started reinterpreting {InputFile}");
                        result = exporter.Run();
                    }
                    writer.Write(result, output, CompressionVersion);
                }
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
            Interpreter? Interpr = null;
            if (!String.IsNullOrEmpty(InterpreterPath))
            {
                using var interpreter_stream = SecureIoHandler.ReadHandleWithInterpreterRedirect(InterpreterPath);
                if (interpreter_stream is null)
                    return -1;
                Interpr = new Interpreter(Interpreter.ToInterpreterDoc(interpreter_stream));
            }

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
            Interpreter Interpr,
            bool overwrite)
        {
            try
            {
                var baseDoc = new XmlDocument();
                String FileNameNew = Path.GetFileNameWithoutExtension(InputFile) + InterpretedFileSuffix + ".xml";

                using (var input = SecureIoHandler.ReadHandle(InputFile))
                using (var output = SecureIoHandler.WriteHandle(FileNameNew, overwrite))
                {
                    if (input is null || output is null)
                        return -1;

                    baseDoc.Load(InputFile);
                    var interpreter = new XmlInterpreter(baseDoc, Interpr);
                    baseDoc = interpreter.Run();
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
            Interpreter? Interpr = null;
            if (!String.IsNullOrEmpty(InterpreterPath))
            {
                using var interpreter_stream = SecureIoHandler.ReadHandleWithInterpreterRedirect(InterpreterPath);
                if (interpreter_stream is null)
                    return -1;
                Interpr = new Interpreter(Interpreter.ToInterpreterDoc(interpreter_stream));
            }

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
                using (var input = SecureIoHandler.WriteHandle(FileNameNew, overwrite))
                using (var output = SecureIoHandler.ReadHandle(InputFile))
                {
                    if (input is null || output is null)
                        return -1;

                    inputDoc.Load(input);
                    var exporter = new XmlExporter(inputDoc, Interpr);
                    var doc = exporter.Run();
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
                    using (var fs = SecureIoHandler.ReadHandle(s))
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
            Interpreter? Interpr = null;
            if (!String.IsNullOrEmpty(InterpreterPath))
            {
                using var interpreter_stream = SecureIoHandler.ReadHandleWithInterpreterRedirect(InterpreterPath);
                if (interpreter_stream is null)
                    return -1;
                Interpr = new Interpreter(Interpreter.ToInterpreterDoc(interpreter_stream));
            }

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

                using (var fs = SecureIoHandler.WriteHandle(FileNameNew, overwrite))
                using (var input = SecureIoHandler.ReadHandle(InputFile))
                {
                    var result = FcFileHelper.ReadFcFile(input);
                    if (Interpr is not null)
                    {
                        var interpreter = new XmlInterpreter(result, Interpr);
                        result = interpreter.Run();
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
            Interpreter? Interpr = null;
            if (!String.IsNullOrEmpty(InterpreterPath))
            {
                using var interpreter_stream = SecureIoHandler.ReadHandleWithInterpreterRedirect(InterpreterPath);
                if (interpreter_stream is null)
                    return -1;
                Interpr = new Interpreter(Interpreter.ToInterpreterDoc(interpreter_stream));
            }

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

                using (var input = SecureIoHandler.ReadHandle(InputFile))
                using (var output = SecureIoHandler.WriteHandle(path, overwrite))
                {
                    if (input is null || output is null)
                        return;

                    XmlDocument exported = new XmlDocument();
                    exported.Load(input);
                    if (Interpr is not null)
                    {
                        var exporter = new XmlExporter(exported, Interpr);
                        exported = exporter.Run();
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
