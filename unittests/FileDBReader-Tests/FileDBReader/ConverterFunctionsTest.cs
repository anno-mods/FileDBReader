using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileDBReader;
using AnnoMods.BBDom.Util;

namespace FileDBReader_Tests
{
    [TestClass, TestCategory("Converter Functions")]
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
        public void TypeConversion_Test_String()
        {
            Assert.IsTrue(StringConversion_Test_Generic(new ASCIIEncoding()));
            Assert.IsTrue(StringConversion_Test_Generic(new UTF7Encoding()));
            Assert.IsTrue(StringConversion_Test_Generic(new UTF8Encoding()));
            Assert.IsTrue(StringConversion_Test_Generic(new UnicodeEncoding()));
            Assert.IsTrue(StringConversion_Test_Generic(new UTF32Encoding()));
        }

        [TestMethod]
        public void InvariantFloatTests()
        {
            String float1 = "1.5";
            var expected = BitConverter.GetBytes(1.5f);
            var parsed = ConverterFunctions.ConversionRulesExport[typeof(float)](float1, Encoding.Unicode);
            
            String float2 = "1,5";
            var parsed2 = ConverterFunctions.ConversionRulesExport[typeof(float)](float2, Encoding.Unicode);
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(parsed2[i], expected[i]);
                Assert.AreEqual(parsed[i], expected[i]);
            }
        }

        #region HelpfulFunctions
        public bool StringConversion_Test_Generic(Encoding e)
        {
            String Value = "Modder's gonna take over the world!!";
            return TypeConversion_Test_Generic<string>(HexHelper.ToBinHex(e.GetBytes(Value)), e);
        }

        public bool TypeConversion_Test_Generic<T>(String Original, Encoding e)
        {
            String Converted = ConverterFunctions.ConversionRulesImport[typeof(T)] (Original, e);
            String ConvertedBack = HexHelper.ToBinHex(ConverterFunctions.ConversionRulesExport[typeof(T)](Converted, e));

            bool b = ConvertedBack.Equals(Original);

            //for everything different from string, test the object conversion rules. We need to rule out anything that is not unicode encoding,
            //since ConverterFunctions uses Unicode for string objects.
            //This is not too bad because ConverterFunctions[typeof(String)] is not really that likely to be called.
            if (e is UnicodeEncoding)
            {
                T Converted_AsObject = (T)ConverterFunctions.ConversionRulesToObject[typeof(T)](Original);
                b = Converted_AsObject.ToString().Equals(Converted);
            }
            return b;
        }
        #endregion
    }
}
