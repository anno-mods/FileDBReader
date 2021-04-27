using System;

namespace Syroot.BinaryData.Core {

  /// <summary>
  /// Represents utilities for working with <see cref="Endian"/> instances.
  /// </summary>
  public static class EndianTools {
    // ---- FIELDS -------------------------------------------------------------------------------------------------

    /// <summary>The <see cref="Endian"/> representing system endianness.</summary>
    public static readonly Endian SystemEndian = BitConverter.IsLittleEndian ? Endian.Little : Endian.Big;

    /// <summary>The <see cref="Endian"/> not representing system endianness.</summary>
    public static readonly Endian NonSystemEndian = BitConverter.IsLittleEndian ? Endian.Big : Endian.Little;
  }
}