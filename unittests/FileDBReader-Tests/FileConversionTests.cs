using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileDBReader;
using System.Xml; 

namespace FileDBReader_Tests
{
    [TestClass]
    public class FileConversionTests
    {
        static readonly String UNITTEST_FILE_DIR = "unittests";
        static readonly String UNITTEST_FILE_DIR_EXPECTED_DIR = "Expected";
        static readonly String UNITTEST_FILE_DIR_TESTFILES_DIR = "Testfiles";
        static readonly String UNITTEST_INTERPRETER_DIR = "unittest_interpreters";

        public bool Test_Decompress(String FileIn, String FileDecompressed)
        {
            throw new NotImplementedException();
        }

        public bool Test_Interpret(String FileIn, String FileOut)
        {
            throw new NotImplementedException();
        }

        public bool Test_Reinterpret(String FileIn, String FileReinterpret)
        {
            throw new NotImplementedException(); 
        }

        public bool Test_Recompress(String FileIn, String FileRecompressed)
        {
            throw new NotImplementedException();
        }
    }
}
