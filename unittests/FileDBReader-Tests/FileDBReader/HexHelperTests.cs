using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileDBReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileDBReader_Tests
{
    [TestClass()]
    public class HexHelperTests
    {
        [TestMethod()]
        public void FlipTest()
        {
            String BinHex = "30002300290020005400650078007400";
            String Flipped = "00740078006500540020002900230030";

            String FlippedByHexHelper = HexHelper.Flip(BinHex);

            Assert.AreEqual(Flipped, FlippedByHexHelper);
        }

        [TestMethod()]
        public void ToBytesTest()
        {
            byte[] bytes = new byte[] { 3, 5, 1, 187, 6, 13, 2, 5, 61, 3, 3, 55, 33, 11, 42, 13, 37 };
            var stream = new MemoryStream(bytes);

            long StreamPosition = stream.Position;
            var bytesByHexHelper = HexHelper.ToBytes(stream);

            CollectionAssert.AreEqual(bytes, bytesByHexHelper);
            Assert.IsTrue(StreamPosition == stream.Position);
        }

        [TestMethod()]
        public void ToBinHexTest()
        {
            byte[] bytes = new byte[] { 3, 5, 1, 187, 6, 13, 2, 5, 61, 3, 3, 55, 33, 11, 42, 13, 37 };

            String expected = "030501BB060D02053D030337210B2A0D25"; 

            String BinHexBytes = HexHelper.ToBinHex(bytes);

            Assert.AreEqual(expected, BinHexBytes);
        }

        [TestMethod()]
        public void BytesFromBinHexTest()
        {
            var expected = new byte[] { 3, 5, 1, 187, 6, 13, 2, 5, 61, 3, 3, 55, 33, 11, 42, 13, 37 };

            String BinHexBytes = "030501BB060D02053D030337210B2A0D25";
            var bytesByHexHelper = HexHelper.ToBytes(BinHexBytes);

            CollectionAssert.AreEqual(expected, bytesByHexHelper);
        }
    }
}