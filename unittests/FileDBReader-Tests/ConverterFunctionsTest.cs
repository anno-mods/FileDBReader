using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileDBReader; 

namespace FileDBReader_Tests
{
    [TestClass]
    public class ConverterFunctionsTest
    {
        private static Encoding DummyEncoding = new UnicodeEncoding();

        [TestMethod]
        public void TypeConversion_Test_Bool() {
            String Value = "01";
            Assert.IsTrue(TypeConversion_Test_Generic<bool>(Value, DummyEncoding));
        }

        [TestMethod]
        public void TypeConversion_Test_Byte() {
            String Value = "FA";
            Assert.IsTrue(TypeConversion_Test_Generic<byte>(Value, DummyEncoding));
        }

        [TestMethod]
        public void TypeConversion_Test_SByte() {
            String Value = "AF";
            Assert.IsTrue(TypeConversion_Test_Generic<sbyte>(Value, DummyEncoding));
        }

        [TestMethod]
        public void TypeConversion_Test_Short() {
            String Value = "EFEF";
            Assert.IsTrue(TypeConversion_Test_Generic<short>(Value, DummyEncoding));
        }

        [TestMethod]
        public void TypeConversion_Test_UShort() {
            String Value = "F102";
            Assert.IsTrue(TypeConversion_Test_Generic<ushort>(Value, DummyEncoding));
        }

        [TestMethod]
        public void TypeConversion_Test_Int32() {
            String Value = "A1E2F4A2";
            Assert.IsTrue(TypeConversion_Test_Generic<int>(Value, DummyEncoding));
        }

        [TestMethod]
        public void TypeConversion_Test_UInt32() {
            String Value = "F1E2F4A2";
            Assert.IsTrue(TypeConversion_Test_Generic<uint>(Value, DummyEncoding));
        }

        [TestMethod]
        public void TypeConversion_Test_Long() {
            String Value = "F1E2F4A2F1E2F4A2";
            Assert.IsTrue(TypeConversion_Test_Generic<long>(Value, DummyEncoding));
        }

        [TestMethod]
        public void TypeConversion_Test_ULong() {
            String Value = "F1E2F4A2F1E2F4A2";
            Assert.IsTrue(TypeConversion_Test_Generic<ulong>(Value, DummyEncoding));
        }

        [TestMethod]
        public void TypeConversion_Test_Float() {
            String Value = "41483A2A";
            Assert.IsTrue(TypeConversion_Test_Generic<float>(Value, DummyEncoding));
        }

        [TestMethod]
        public void TypeConversion_Test_Double() {
            String Value = "4093407FEF33273A";
            Assert.IsTrue(TypeConversion_Test_Generic<double>(Value, DummyEncoding));
        }

        [TestMethod]
        public void TypeConversion_Test_String_Ascii()
        {
            Assert.IsTrue(StringConversion_Test_Generic(new ASCIIEncoding()));
        }

        [TestMethod]
        //Let's keep this for now, simply because a BDU might still use it ._.
        public void TypeConversion_Test_String_UTF7()
        {
            Assert.IsTrue(StringConversion_Test_Generic(new UTF7Encoding()));
        }

        [TestMethod]
        public void TypeConversion_Test_String_UTF8()
        {
            Assert.IsTrue(StringConversion_Test_Generic(new UTF8Encoding()));
        }

        [TestMethod]
        public void TypeConversion_Test_String_Unicode()
        {
            Assert.IsTrue(StringConversion_Test_Generic(new UnicodeEncoding()));
        }

        [TestMethod]
        public void TypeConversion_Test_String_UTF32()
        {
            Assert.IsTrue(StringConversion_Test_Generic(new UTF32Encoding()));
        }

        #region HelpfulFunctions
        public bool StringConversion_Test_Generic(Encoding e)
        {
            String Value = "Modder's gonna take over the world!!";
            return TypeConversion_Test_Generic<string>(HexHelper.ByteArrayToString(e.GetBytes(Value)), e);
        }

        public bool TypeConversion_Test_Generic<T>(String Original, Encoding e)
        {
            String Converted = ConverterFunctions.ConversionRulesImport[typeof(T)] (Original, e);
            String ConvertedBack = HexHelper.ByteArrayToString(ConverterFunctions.ConversionRulesExport[typeof(T)](Converted, e));

            bool b = ConvertedBack.Equals(Original);
            //for everything different from string, test the object conversion rules
            if (typeof(T) != typeof(String))
            {
                T Converted_AsObject = (T)ConverterFunctions.ConversionRulesToObject[typeof(T)](Original);
                b = Converted_AsObject.ToString().Equals(Converted);
            }
            return b;
        }
        #endregion
    }
}
