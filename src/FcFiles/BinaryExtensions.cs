using System;
using System.IO;
using System.Text;

namespace FileDBReader {

  public static class BinaryExtensions {

    #region Methods

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