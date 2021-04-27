using Syroot.BinaryData.Core;
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Syroot.BinaryData.Memory {

  /// <summary>
  /// Represents a cursor in a <see cref="ReadOnlySpan{Byte}"/> at which data is read from.
  /// </summary>
  [DebuggerDisplay(nameof(SpanReader) + ", Position={Position}")]
  public ref struct SpanReader {
    // ---- FIELDS -------------------------------------------------------------------------------------------------

    private bool _reverseEndian;
    private Encoding _encoding;

    // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="SpanReader"/> struct to operate on the given
    /// <paramref name="span"/>.
    /// </summary>
    /// <param name="span">The <see cref="ReadOnlySpan{Byte}"/> to read from.</param>
    /// <param name="endian">The <see cref="Endian"/> to use.</param>
    /// <param name="encoding">The character encoding to use. Defaults to <see cref="Encoding.UTF8"/>.</param>
    public SpanReader(ReadOnlySpan<byte> span, Endian endian = Endian.System, Encoding encoding = null) {
      _reverseEndian = !endian.IsSystem();
      _encoding = encoding ?? Encoding.UTF8;
      Position = 0;
      Span = span;
    }

    // ---- PROPERTIES ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Gets or sets the encoding used for string related operations where no other encoding has been provided.
    /// Setting this value to <see langword="null"/> will restore the default <see cref="Encoding.UTF8"/>.
    /// </summary>
    public Encoding Encoding {
      get => _encoding;
      set => _encoding = value ?? Encoding.UTF8;
    }

    /// <summary>
    /// Gets or sets the <see cref="Endian"/> used to parse multibyte binary data with.
    /// </summary>
    public Endian Endian {
      get => _reverseEndian ? EndianTools.NonSystemEndian : EndianTools.SystemEndian;
      set => _reverseEndian = !value.IsSystem();
    }

    /// <summary>
    /// Gets a value indicating whether the current position reached the end of the span.
    /// </summary>
    public bool IsEndOfSpan => Span.Length - Position <= 0;

    /// <summary>
    /// Gets the length of the underlying <see cref="Span"/>.
    /// </summary>
    public int Length => Span.Length;

    /// <summary>
    /// Gets or sets the index in the <see cref="Span"/> at which the next data is read from.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Gets the <see cref="ReadOnlySpan{Byte}"/> from which data is read.
    /// </summary>
    public ReadOnlySpan<byte> Span { get; }

    // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

    /// <summary>
    /// Aligns the position to the given byte multiple.
    /// </summary>
    /// <param name="alignment">The byte multiple to align to. If negative, the position is decreased to the
    /// previous multiple rather than the next one.</param>
    /// <returns>The new position.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Align(int alignment) => Position += MathTools.GetAlignmentDelta(Position, alignment);

    /// <summary>
    /// Reads a 1-byte <see cref="Boolean"/> value from the current position. This is the .NET default format.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Boolean ReadBoolean() => Span[Position++] != 0;

    /// <summary>
    /// Reads a 1-byte <see cref="Boolean"/> value from the current position. This is the .NET default format.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadBoolean(out Boolean result) => result = Span[Position++] != 0;

    /// <summary>
    /// Reads a 2-byte <see cref="Boolean"/> value from the current position.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Boolean ReadBoolean2() {
      var result = MemoryMarshal.Read<UInt16>(Span.Slice(Position)) != 0;
      Position += sizeof(UInt16);
      return result;
    }

    /// <summary>
    /// Reads a 2-byte <see cref="Boolean"/> value from the current position.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadBoolean2(out Boolean result) {
      result = MemoryMarshal.Read<UInt16>(Span.Slice(Position)) != 0;
      Position += sizeof(UInt16);
    }

    /// <summary>
    /// Reads a 4-byte <see cref="Boolean"/> value from the current position.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Boolean ReadBoolean4() {
      var result = MemoryMarshal.Read<UInt32>(Span.Slice(Position)) != 0;
      Position += sizeof(UInt32);
      return result;
    }

    /// <summary>
    /// Reads a 4-byte <see cref="Boolean"/> value from the current position.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadBoolean4(out Boolean result) {
      result = MemoryMarshal.Read<UInt32>(Span.Slice(Position)) != 0;
      Position += sizeof(UInt32);
    }

    /// <summary>
    /// Reads the given number of <see cref="Byte"/> values from the current position.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Byte[] ReadBytes(int count) {
      byte[] value = Span.Slice(Position, count).ToArray();
      Position += count;
      return value;
    }

    /// <summary>
    /// Reads a <see cref="Byte"/> value from the current position.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Byte ReadByte() => Span[Position++];

    /// <summary>
    /// Reads a <see cref="Byte"/> value from the current position.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    public void ReadByte(out Byte result) => result = Span[Position++];

    /// <summary>
    /// Reads a <see cref="DateTime"/> value from the current position, stored as the ticks of a .NET
    /// <see cref="DateTime"/> instance.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTime ReadDateTime() => new DateTime(ReadInt64());

    /// <summary>
    /// Reads a <see cref="DateTime"/> value from the current position, stored as the ticks of a .NET
    /// <see cref="DateTime"/> instance.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadDateTime(out DateTime result) => result = new DateTime(ReadInt64());

    /// <summary>
    /// Reads a <see cref="DateTime"/> value from the current position, stored in the 32-bit time_t format of the C
    /// library. This is a <see cref="UInt32"/> which can store the seconds from 1970-01-01 until approx.
    /// 2106-02-07.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTime ReadDateTimeT() => CTimeTools.GetDateTime(ReadUInt32());

    /// <summary>
    /// Reads a <see cref="DateTime"/> value from the current position, stored in the 32-bit time_t format of the C
    /// library. This is a <see cref="UInt32"/> which can store the seconds from 1970-01-01 until approx.
    /// 2106-02-07.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadDateTimeT(out DateTime result) => result = CTimeTools.GetDateTime(ReadUInt32());

    /// <summary>
    /// Reads a <see cref="DateTime"/> value from the current position, stored in the 64-bit time_t format of the C
    /// library. This is an <see cref="Int64"/> which can store the seconds from 1970-01-01 until approx.
    /// 292277026596-12-04.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTime ReadDateTimeT64() => CTimeTools.GetDateTime(ReadUInt64());

    /// <summary>
    /// Reads a <see cref="DateTime"/> value from the current position, stored in the 64-bit time_t format of the C
    /// library. This is an <see cref="Int64"/> which can store the seconds from 1970-01-01 until approx.
    /// 292277026596-12-04.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadDateTimeT64(out DateTime result) => result = CTimeTools.GetDateTime(ReadUInt64());

    /// <summary>
    /// Reads a <see cref="Decimal"/> value from the current position.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Decimal ReadDecimal() {
      var value = MemoryMarshal.Read<Decimal>(Span.Slice(Position));
      Position += sizeof(Decimal);
      return value;
    }

    /// <summary>
    /// Reads a <see cref="Decimal"/> value from the current position.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadDecimal(out Decimal result) {
      result = MemoryMarshal.Read<Decimal>(Span.Slice(Position));
      Position += sizeof(Decimal);
    }

    /// <summary>
    /// Reads a <see cref="Double"/> value from the current position.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Double ReadDouble() {
      var raw = ReadUInt64();
      return *(Double*)&raw;
    }

    /// <summary>
    /// Reads a <see cref="Double"/> value from the current position.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void ReadDouble(out Double result) {
      var raw = ReadUInt64();
      result = *(Double*)&raw;
    }

    /// <summary>
    /// Reads an <see cref="Enum"/> value from the current position, using the size of the underlying type.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ReadEnum<T>() where T : struct, Enum {
      ReadEnum(out T result);
      return result;
    }

    /// <summary>
    /// Reads an <see cref="Enum"/> value from the current position, using the size of the underlying type.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadEnum<T>(out T result) where T : struct, Enum {
      int length = Unsafe.SizeOf<T>();
      Span<byte> raw = stackalloc byte[length];
      Span.Slice(Position, length).CopyTo(raw);
      Position += length;
      if (_reverseEndian)
        raw.Reverse();
      result = MemoryMarshal.Read<T>(raw);
    }

    /// <summary>
    /// Reads an <see cref="Enum"/> value from the current position, using the size of the underlying type. The
    /// value is validated, throwing an exception if it is not defined in the enum.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentException">The value is not defined in the enum.</exception>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ReadEnumSafe<T>() where T : struct, Enum {
      ReadEnumSafe(out T result);
      return result;
    }

    /// <summary>
    /// Reads an <see cref="Enum"/> value from the current position, using the size of the underlying type. The
    /// value is validated, throwing an exception if it is not defined in the enum.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentException">The value is not defined in the enum.</exception>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadEnumSafe<T>(out T result) where T : struct, Enum {
      result = ReadEnum<T>();
      if (!EnumTools.Validate(typeof(T), result))
        throw new ArgumentException($"Value {result} is not valid for enum {typeof(T)}.");
    }

    /// <summary>
    /// Reads an <see cref="Int16"/> value from the current position.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Int16 ReadInt16(bool setPosition = true) {
      var value = MemoryMarshal.Read<Int16>(Span.Slice(Position));
      if (setPosition) {
        Position += sizeof(Int16);
      }
      return _reverseEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    /// <summary>
    /// Reads an <see cref="Int16"/> value from the current position.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadInt16(out Int16 result) {
      result = _reverseEndian
          ? BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<Int16>(Span.Slice(Position)))
          : MemoryMarshal.Read<Int16>(Span.Slice(Position));
      Position += sizeof(Int16);
    }

    /// <summary>
    /// Reads an <see cref="Int32"/> value from the current position.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Int32 ReadInt32() {
      var value = MemoryMarshal.Read<Int32>(Span.Slice(Position));
      Position += sizeof(Int32);
      return _reverseEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    /// <summary>
    /// Reads an <see cref="Int32"/> value from the current position.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadInt32(out Int32 result) {
      result = _reverseEndian
          ? BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<Int32>(Span.Slice(Position)))
          : MemoryMarshal.Read<Int32>(Span.Slice(Position));
      Position += sizeof(Int32);
    }

    /// <summary>
    /// Reads an <see cref="Int32"/> value from the current position. The value is stored in 1 to 5 bytes, only
    /// using another byte if it does not fit into 7 more bits of the current one.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentException">The data available is not a 7-bit encoded integer.</exception>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Int32 ReadInt32Bit7() {
      ReadInt32Bit7(out var result);
      return result;
    }

    /// <summary>
    /// Reads an <see cref="Int32"/> value from the current position. The value is stored in 1 to 5 bytes, only
    /// using another byte if it does not fit into 7 more bits of the current one.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentException">The data available is not a 7-bit encoded integer.</exception>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadInt32Bit7(out Int32 result) {
      // Endianness does not matter, as this value is stored byte by byte.
      // While the highest bit is set, the integer requires another of a maximum of 5 bytes.
      result = 0;
      for (int i = 0; i < sizeof(Int32) + 1; i++) {
        byte readByte = Span[Position++];
        result |= (readByte & 0b01111111) << i * 7;
        if ((readByte & 0b10000000) == 0)
          return;
      }
      throw new ArgumentException("Invalid 7-bit encoded Int32.");
    }

    /// <summary>
    /// Reads an <see cref="Int64"/> value from the current position.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Int64 ReadInt64() {
      var value = MemoryMarshal.Read<Int64>(Span.Slice(Position));
      Position += sizeof(Int64);
      return _reverseEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    /// <summary>
    /// Reads an <see cref="Int64"/> value from the current position.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadInt64(out Int64 result) {
      result = _reverseEndian
          ? BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<Int64>(Span.Slice(Position)))
          : MemoryMarshal.Read<Int64>(Span.Slice(Position));
      Position += sizeof(Int64);
    }

    /// <summary>
    /// Reads an <see cref="SByte"/> value from the current position.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SByte ReadSByte() => (SByte)Span[Position++];

    /// <summary>
    /// Reads an <see cref="SByte"/> value from the current position.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadSByte(out SByte result) => result = (SByte)Span[Position++];

    /// <summary>
    /// Reads an <see cref="Single"/> value from the current position.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Single ReadSingle() {
      var raw = ReadUInt32();
      return *(Single*)&raw;
    }

    /// <summary>
    /// Reads an <see cref="Single"/> value from the current position.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void ReadSingle(out Single result) {
      var raw = ReadUInt32();
      result = *(Single*)&raw;
    }

    /// <summary>
    /// Reads a <see cref="String"/> value from the current position. It has a prefix of a 7-bit encoded integer of
    /// variable size determining the number of bytes out of which the string consists, and no postfix. This is the
    /// .NET <see cref="System.IO.BinaryReader"/> and <see cref="System.IO.BinaryWriter"/> default format.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public String ReadString() {
      ReadString(out var result);
      return result;
    }

    /// <summary>
    /// Reads a <see cref="String"/> value from the current position. It has a prefix of a 7-bit encoded integer of
    /// variable size determining the number of bytes out of which the string consists, and no postfix. This is the
    /// .NET <see cref="System.IO.BinaryReader"/> and <see cref="System.IO.BinaryWriter"/> default format.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadString(out String result) {
      int length = ReadInt32Bit7();
#if NETCOREAPP2_1
      result = _encoding.GetString(Span.Slice(Position, length));
#else
            unsafe
            {
                fixed (byte* pSpan = Span.Slice(Position))
                    result = _encoding.GetString(pSpan, length);
            }
#endif
      Position += length;
    }

    /// <summary>
    /// Reads a <see cref="String"/> value from the current position. It has no prefix and is terminated with a 0
    /// value. The size of this value depends on the encoding.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public String ReadString0() {
      ReadString0(out var result);
      return result;
    }

    /// <summary>
    /// Reads a <see cref="String"/> value from the current position. It has no prefix and is terminated with a 0
    /// value. The size of this value depends on the encoding.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadString0(out String result) {
      // Read values until a 0 value is found (no encoding's char surrogate should consist of 0 completely).
      // Endianness depends on encoding, not the actual values.
      int length = 0;
      int terminatorSize = _encoding.GetByteCount("A");
      for (byte lastByte = 1; lastByte != 0; length += terminatorSize) {
        for (int i = 0; i < terminatorSize; i++) {
          lastByte = Span[Position + length + i];
          if (lastByte != 0)
            break;
        }
      }
      // Return the string up to the terminator.
#if NETCOREAPP2_1
      result = _encoding.GetString(Span.Slice(Position, length - terminatorSize));
#else
            unsafe
            {
                fixed (byte* pSpan = Span.Slice(Position))
                    result = _encoding.GetString(pSpan, length - terminatorSize);
            }
#endif
      Position += length;
    }

    /// <summary>
    /// Reads a <see cref="String"/> value from the current position. It has a <see cref="Byte"/> prefix
    /// determining the number of chars out of which the string consists, and no postfix.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public String ReadString1() {
      DecodeString(out var result, ReadByte());
      return result;
    }

    /// <summary>
    /// Reads a <see cref="String"/> value from the current position. It has a <see cref="Byte"/> prefix
    /// determining the number of chars out of which the string consists, and no postfix.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadString1(out String result) => DecodeString(out result, ReadByte());

    /// <summary>
    /// Reads a <see cref="String"/> value from the current position. It has a <see cref="UInt16"/> prefix
    /// determining the number of chars out of which the string consists, and no postfix.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public String ReadString2() {
      DecodeString(out var result, ReadUInt16());
      return result;
    }

    /// <summary>
    /// Reads a <see cref="String"/> value from the current position. It has a <see cref="UInt16"/> prefix
    /// determining the number of chars out of which the string consists, and no postfix.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadString2(out String result) => DecodeString(out result, ReadUInt16());

    /// <summary>
    /// Reads a <see cref="String"/> value from the current position. It has an <see cref="Int32"/> prefix
    /// determining the number of chars out of which the string consists, and no postfix.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public String ReadString4() {
      DecodeString(out var result, ReadInt32());
      return result;
    }

    /// <summary>
    /// Reads a <see cref="String"/> value from the current position. It has an <see cref="Int32"/> prefix
    /// determining the number of chars out of which the string consists, and no postfix.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadString4(out String result) => DecodeString(out result, ReadInt32());

    /// <summary>
    /// Reads a <see cref="String"/> value from the current position. It has neither prefix nor postfix, and is
    /// stored in a buffer with the given <paramref name="byteCount"/>, padded with 0 if it is longer than the
    /// string data.
    /// </summary>
    /// <param name="byteCount">The number of bytes to read the value from.</param>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public String ReadStringFix(int byteCount) {
      ReadStringFix(out var result, byteCount);
      return result;
    }

    /// <summary>
    /// Reads a <see cref="String"/> value from the current position. It has neither prefix nor postfix, and is
    /// stored in a buffer with the given <paramref name="byteCount"/>, padded with 0 if it is longer than the
    /// string data.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <param name="byteCount">The number of bytes to read the value from.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadStringFix(out String result, int byteCount) {
#if NETCOREAPP2_1
      result = _encoding.GetString(Span.Slice(Position, byteCount));
#else
            unsafe
            {
                fixed (byte* pSpan = Span.Slice(Position))
                    result = _encoding.GetString(pSpan, byteCount);
            }
#endif
      Position += byteCount;
      result = result.TrimEnd('\0');
    }

    /// <summary>
    /// Reads a <see cref="String"/> value from the current position. It has neither prefix nor postfix, the given
    /// <paramref name="length"/> is expected.
    /// </summary>
    /// <param name="length">The length of the string, in characters.</param>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public String ReadStringRaw(int length) {
      DecodeString(out var result, length);
      return result;
    }

    /// <summary>
    /// Reads a <see cref="String"/> value from the current position. It has neither prefix nor postfix, the given
    /// <paramref name="length"/> is expected.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <param name="length">The length of the string, in characters.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadStringRaw(out String result, int length) => DecodeString(out result, length);

    /// <summary>
    /// Reads a <see cref="UInt16"/> value from the current position.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UInt16 ReadUInt16() {
      var value = MemoryMarshal.Read<UInt16>(Span.Slice(Position));
      Position += sizeof(UInt16);
      return _reverseEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    /// <summary>
    /// Reads a <see cref="UInt16"/> value from the current position.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadUInt16(out UInt16 result) {
      result = _reverseEndian
          ? BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<UInt16>(Span.Slice(Position)))
          : MemoryMarshal.Read<UInt16>(Span.Slice(Position));
      Position += sizeof(UInt16);
    }

    /// <summary>
    /// Reads a <see cref="UInt32"/> value from the current position.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UInt32 ReadUInt32() {
      var value = MemoryMarshal.Read<UInt32>(Span.Slice(Position));
      Position += sizeof(UInt32);
      return _reverseEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    /// <summary>
    /// Reads a <see cref="UInt32"/> value from the current position.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadUInt32(out UInt32 result) {
      result = _reverseEndian
          ? BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<UInt32>(Span.Slice(Position)))
          : MemoryMarshal.Read<UInt32>(Span.Slice(Position));
      Position += sizeof(UInt32);
    }

    /// <summary>
    /// Reads a <see cref="UInt64"/> value from the current position.
    /// </summary>
    /// <returns>The value retrieved from the current position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UInt64 ReadUInt64() {
      var value = MemoryMarshal.Read<UInt64>(Span.Slice(Position));
      Position += sizeof(UInt64);
      return _reverseEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    /// <summary>
    /// Reads a <see cref="UInt64"/> value from the current position.
    /// </summary>
    /// <param name="result">The value retrieved from the current position.</param>
    /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadUInt64(out UInt64 result) {
      result = _reverseEndian
          ? BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<UInt64>(Span.Slice(Position)))
          : MemoryMarshal.Read<UInt64>(Span.Slice(Position));
      Position += sizeof(UInt64);
    }

    /// <summary>
    /// Gets a <see cref="SpanReader"/> for the remaining data following the current position.
    /// </summary>
    /// <returns>The <see cref="SpanReader"/> covering the remaining data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SpanReader Slice() => new SpanReader(Span.Slice(Position));

    /// <summary>
    /// Gets a <see cref="SpanReader"/> for the data covered from the given <paramref name="start"/> position.
    /// </summary>
    /// <param name="start">The position from which to start covering data.</param>
    /// <returns>The <see cref="SpanReader"/> covering the remaining data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SpanReader Slice(int start) => new SpanReader(Span.Slice(start));

    /// <summary>
    /// Gets a <see cref="SpanReader"/> for the data covered by the given <paramref name="start"/> position and
    /// <paramref name="length"/>.
    /// </summary>
    /// <param name="start">The position from which to start covering data.</param>
    /// <param name="length">The number of bytes to cover.</param>
    /// <returns>The <see cref="SpanReader"/> covering the remaining data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SpanReader Slice(int start, int length) => new SpanReader(Span.Slice(start, length));

    // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DecodeString(out String result, int length) {
      if (length == 0)
        result = String.Empty;

      var decoder = _encoding.GetDecoder();
      Span<char> chars = stackalloc char[length];
#if NETCOREAPP2_1
      decoder.Convert(Span.Slice(Position), chars, true, out int bytesUsed, out int charsUsed, out bool completed);
#else
            int bytesUsed;
            unsafe
            {
                fixed (byte* pSpan = Span.Slice(Position))
                {
                    fixed (char* pChars = chars)
                    {
                        decoder.Convert(pSpan, Span.Length - Position, pChars, length, true, out bytesUsed,
                            out int charsUsed, out bool completed);
                    }
                }
            }
#endif
      Position += bytesUsed;
      result = chars.ToString();
    }
  }
}