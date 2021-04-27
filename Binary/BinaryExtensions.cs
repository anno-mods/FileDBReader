using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;
using System;
using System.Text;

namespace FileDBReader {

  public static class BinaryExtensions {

    #region Methods

    public static SpanReader ToReader(this ReadOnlySpan<byte> span, Endian endian = Endian.System, Encoding encoding = null) {
      return new SpanReader(span, endian, encoding);
    }

    public static SpanReader ToReader(this ReadOnlySpan<byte> span, out SpanReader reader, Endian endian = Endian.System, Encoding encoding = null) {
      reader = new SpanReader(span, endian, encoding);
      return reader;
    }

    public static string ToHexString(this ReadOnlySpan<byte> Bytes) {
      StringBuilder Result = new StringBuilder(Bytes.Length * 2);
      string HexAlphabet = "0123456789ABCDEF";

      foreach (var B in Bytes) {
        Result.Append(HexAlphabet[B >> 4]);
        Result.Append(HexAlphabet[B & 0xF]);
      }

      return Result.ToString();
    }

    #endregion Methods
  }
}