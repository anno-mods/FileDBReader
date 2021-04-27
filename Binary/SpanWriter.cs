using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Syroot.BinaryData.Core;

namespace Syroot.BinaryData.Memory
{
    /// <summary>
    /// Represents a cursor in a <see cref="Span{Byte}"/> at which data is written to.
    /// </summary>
    [DebuggerDisplay(nameof(SpanWriter) + ", Position={Position}")]
    public ref struct SpanWriter
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private bool _reverseEndian;
        private Encoding _encoding;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanWriter"/> struct to operate on the given
        /// <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The <see cref="Span{Byte}"/> to write to.</param>
        /// <param name="endian">The <see cref="Endian"/> to use.</param>
        /// <param name="encoding">The character encoding to use. Defaults to <see cref="Encoding.UTF8"/>.</param>
        public SpanWriter(Span<byte> span, Endian endian = Endian.System, Encoding encoding = null)
        {
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
        public Encoding Encoding
        {
            get => _encoding;
            set => _encoding = value ?? Encoding.UTF8;
        }

        /// <summary>
        /// Gets or sets the <see cref="Endian"/> used to parse multibyte binary data with.
        /// </summary>
        public Endian Endian
        {
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
        /// Gets or sets the index in the <see cref="Span"/> at which the next data is written to.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Span{Byte}"/> to which data is written.
        /// </summary>
        public Span<byte> Span { get; }

        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Implicitly converts the <see cref="SpanWriter"/> to a <see cref="SpanReader"/> instance.
        /// </summary>
        /// <param name="spanWriter">The <see cref="SpanWriter"/> to convert.</param>
        public static implicit operator SpanReader(SpanWriter spanWriter)
            => new SpanReader(spanWriter.Span, spanWriter.Endian, spanWriter.Encoding);

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Aligns the cursor to the given byte multiple.
        /// </summary>
        /// <param name="alignment">The byte multiple to align to. If negative, the position is decreased to the
        /// previous multiple rather than the next one.</param>
        /// <returns>The new position.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Align(int alignment) => Position += MathTools.GetAlignmentDelta(Position, alignment);

        /// <summary>
        /// Gets a <see cref="SpanReader"/> for the remaining data following the current position.
        /// </summary>
        /// <returns>The <see cref="SpanReader"/> covering the remaining data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanWriter Slice() => new SpanWriter(Span.Slice(Position, Span.Length - Position));

        /// <summary>
        /// Gets a <see cref="SpanWriter"/> for the data covered from the given <paramref name="start"/> position.
        /// </summary>
        /// <param name="start">The position from which to start covering data.</param>
        /// <returns>The <see cref="SpanWriter"/> covering the remaining data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanWriter Slice(int start) => new SpanWriter(Span.Slice(start, Span.Length - start));

        /// <summary>
        /// Gets a <see cref="SpanWriter"/> for the data covered by the given <paramref name="start"/> position and
        /// <paramref name="length"/>.
        /// </summary>
        /// <param name="start">The position from which to start covering data.</param>
        /// <param name="length">The number of bytes to cover.</param>
        /// <returns>The <see cref="SpanWriter"/> covering the remaining data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanWriter Slice(int start, int length) => new SpanWriter(Span.Slice(start, length));

        /// <summary>
        /// Writes a 1-byte <see cref="Boolean"/> at the current position. This is the .NET default format.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBoolean(Boolean value) => Span[Position++] = (Byte)(value ? 1 : 0);

        /// <summary>
        /// Writes a 2-byte <see cref="Boolean"/> at the current position.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBoolean2(Boolean value)
        {
      var raw = _reverseEndian
                ? (UInt16)(value ? 0b00000001_00000000 : 0)
                : (UInt16)(value ? 0b00000000_00000001 : 0);
            MemoryMarshal.Write(Span.Slice(Position), ref raw);
            Position += sizeof(UInt16);
        }

        /// <summary>
        /// Writes a 4-byte <see cref="Boolean"/> at the current position.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBoolean4(Boolean value)
        {
      var raw = _reverseEndian
                ? (UInt32)(value ? 0b00000001_00000000_00000000_00000000 : 0)
                : (UInt32)(value ? 0b00000000_00000000_00000000_00000001 : 0);
            MemoryMarshal.Write(Span.Slice(Position), ref raw);
            Position += sizeof(UInt32);
        }

        /// <summary>
        /// Writes <see cref="Byte"/> values at the current position.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(ReadOnlySpan<Byte> value)
        {
            value.CopyTo(Span.Slice(Position));
            Position += value.Length;
        }

        /// <summary>
        /// Writes a <see cref="Byte"/> at the current position.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(Byte value) => Span[Position++] = value;

        /// <summary>
        /// Writes a <see cref="DateTime"/> at the current position, stored as the ticks of a .NET
        /// <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDateTime(DateTime value) => WriteInt64(value.Ticks);

        /// <summary>
        /// Writes a <see cref="DateTime"/> at the current position, stored in the 32-bit time_t format of the C
        /// library. This is a <see cref="UInt32"/> which can store the seconds from 1970-01-01 until approx.
        /// 2106-02-07.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDateTimeT(DateTime value) => WriteUInt32((UInt32)CTimeTools.GetSeconds(value));

        /// <summary>
        /// Writes a <see cref="DateTime"/> at the current position, stored in the 64-bit time_t format of the C
        /// library. This is an <see cref="Int64"/> which can store the seconds from 1970-01-01 until approx.
        /// 292277026596-12-04.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDateTimeT64(DateTime value) => WriteUInt64((UInt64)CTimeTools.GetSeconds(value));

        /// <summary>
        /// Writes a <see cref="Decimal"/> at the current position.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDecimal(Decimal value)
        {
            MemoryMarshal.Write(Span.Slice(Position), ref value);
            Position += sizeof(Decimal);
        }

        /// <summary>
        /// Writes a <see cref="Double"/> at the current position.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void WriteDouble(Double value) => WriteUInt64(*(UInt64*)&value);

        /// <summary>
        /// Writes an <see cref="Enum"/> at the current position, using the size of the underlying type.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void WriteEnum<T>(T value) where T : struct, Enum
        {
      var raw = new Span<byte>(Unsafe.AsPointer(ref value), Unsafe.SizeOf<T>());
            if (_reverseEndian)
                raw.Reverse();
            WriteBytes(raw);
        }

        /// <summary>
        /// Writes an <see cref="Enum"/> at the current position, using the size of the underlying type. The value is
        /// validated, throwing an exception if it is not defined in the enum.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentException">The value is not defined in the enum.</exception>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteEnumSafe<T>(T value) where T : struct, Enum
        {
            if (!EnumTools.Validate(typeof(T), value))
                throw new ArgumentException($"Value {value} is not valid for enum {typeof(T)}.");
            WriteEnum(value);
        }

        /// <summary>
        /// Writes an <see cref="Int16"/> at the current position.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt16(Int16 value)
        {
            if (_reverseEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            MemoryMarshal.Write(Span.Slice(Position), ref value);
            Position += sizeof(Int16);
        }

        /// <summary>
        /// Writes an <see cref="Int32"/> at the current position.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt32(Int32 value)
        {
            if (_reverseEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            MemoryMarshal.Write(Span.Slice(Position), ref value);
            Position += sizeof(Int32);
        }

        /// <summary>
        /// Writes an <see cref="Int32"/> at the current position. The value is stored in 1 to 5 bytes, only using
        /// another byte if it does not fit into 7 more bits of the current one.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt32Bit7(Int32 value)
        {
      // The highest bit determines whether to continue writing more bytes to form the Int32 value.
      var unsigned = (UInt32)value;
            while (unsigned >= 0b10000000)
            {
                Span[Position++] = (byte)(unsigned | 0b10000000);
                unsigned >>= 7;
            }
            Span[Position++] = (byte)unsigned;
        }

        /// <summary>
        /// Writes an <see cref="Int64"/> at the current position.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt64(Int64 value)
        {
            if (_reverseEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            MemoryMarshal.Write(Span.Slice(Position), ref value);
            Position += sizeof(Int64);
        }

        /// <summary>
        /// Writes an <see cref="SByte"/> at the current position.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSByte(SByte value) => Span[Position++] = (Byte)value;

        /// <summary>
        /// Writes a <see cref="Single"/> at the current position.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void WriteSingle(Single value) => WriteUInt32(*(UInt32*)&value);

        /// <summary>
        /// Writes a <see cref="String"/> at the current position. It has a prefix of a 7-bit encoded integer of
        /// variable size determining the number of bytes out of which the string consists, and no postfix. This is the
        /// .NET <see cref="System.IO.BinaryReader"/> and <see cref="System.IO.BinaryWriter"/> default format.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(String value)
        {
            byte[] raw = Encoding.GetBytes(value);
            WriteInt32Bit7(raw.Length);
            WriteBytes(raw);
        }

        /// <summary>
        /// Writes a <see cref="String"/> at the current position. It has no prefix and is terminated with a 0 value.
        /// The size of this value depends on the encoding.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString0(String value)
        {
            WriteBytes(Encoding.GetBytes(value));
            switch (_encoding.GetByteCount("A"))
            {
                case sizeof(Byte):
                    WriteByte(0);
                    break;
                case sizeof(Int16):
                    WriteByte(0);
                    WriteByte(0);
                    break;
                case sizeof(Int32):
                    WriteByte(0);
                    WriteByte(0);
                    WriteByte(0);
                    WriteByte(0);
                    break;
            }
        }

        /// <summary>
        /// Writes a <see cref="String"/> at the current position. It has a <see cref="Byte"/> prefix determining the
        /// number of chars out of which the string consists and no postfix.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString1(String value)
        {
            WriteByte((Byte)value.Length);
            WriteBytes(Encoding.GetBytes(value));
        }

        /// <summary>
        /// Writes a <see cref="String"/> at the current position. It has a <see cref="UInt16"/> prefix determining the
        /// number of chars out of which the string consists, and no postfix.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString2(String value)
        {
            WriteUInt16((UInt16)value.Length);
            WriteBytes(Encoding.GetBytes(value));
        }

        /// <summary>
        /// Writes a <see cref="String"/> at the current position. It has an <see cref="Int32"/> prefix determining the
        /// number of chars out of which the string consists, and no postfix.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString4(String value)
        {
            WriteInt32(value.Length);
            WriteBytes(Encoding.GetBytes(value));
        }

        /// <summary>
        /// Writes a <see cref="String"/> at the current position. It has neither prefix nor postfix, and is stored in
        /// a buffer with the given <paramref name="byteCount"/>, padded with 0 if it is longer than the string data.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <param name="byteCount">The number of bytes to store the value in.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteStringFix(String value, int byteCount)
        {
            byte[] raw = Encoding.GetBytes(value);
            if (raw.Length > byteCount)
                throw new ArgumentOutOfRangeException($"The string data does not fit into {byteCount} bytes.");
            Span.Slice(Position, byteCount).Clear();
            ((ReadOnlySpan<byte>)raw).CopyTo(Span.Slice(Position));
            Position += byteCount;
        }

        /// <summary>
        /// Writes a <see cref="String"/> at the current position. It has neither prefix nor postfix.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteStringRaw(String value) => WriteBytes(Encoding.GetBytes(value));

#if NETCOREAPP2_1
        /// <summary>
        /// Writes a <see cref="ReadOnlySpan{Char}"/> at the current position. It has a prefix of a 7-bit encoded
        /// <see cref="Int32"/> determining the number of bytes out of which the string consists, and no postfix. This
        /// is the .NET <see cref="System.IO.BinaryReader"/> and <see cref="System.IO.BinaryWriter"/> default.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(ReadOnlySpan<char> value)
        {
            // Span has to be run through the encoder twice:
            // Once to get the exact number of bytes to write the length, another time to store the bytes in the span.
            // The space for the length prefix cannot be reserved in advance as it is of variable length.
            int length = Encoding.GetByteCount(value);
            WriteInt32Bit7(length);
            Encoding.GetBytes(value, Span.Slice(Position));
            Position += length;
        }

        /// <summary>
        /// Writes a <see cref="ReadOnlySpan{Char}"/> at the current position. It has no prefix and is terminated with a
        /// 0 value. The size of this value depends on the encoding.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString0(ReadOnlySpan<char> value)
        {
            Position += Encoding.GetBytes(value, Span.Slice(Position));
            switch (_encoding.GetByteCount("A"))
            {
                case sizeof(Byte):
                    WriteByte(0);
                    break;
                case sizeof(Int16):
                    WriteByte(0);
                    WriteByte(0);
                    break;
                case sizeof(Int32):
                    WriteByte(0);
                    WriteByte(0);
                    WriteByte(0);
                    WriteByte(0);
                    break;
            }
        }

        /// <summary>
        /// Writes a <see cref="ReadOnlySpan{Char}"/> at the current position. It has a <see cref="Byte"/> prefix
        /// determining the number of chars out of which the string consists and no postfix.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString1(ReadOnlySpan<char> value)
        {
            WriteByte((Byte)value.Length);
            Position += Encoding.GetBytes(value, Span.Slice(Position));
        }

        /// <summary>
        /// Writes a <see cref="ReadOnlySpan{Char}"/> at the current position. It has a <see cref="UInt16"/> prefix
        /// determining the number of chars out of which the string consists, and no postfix.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString2(ReadOnlySpan<char> value)
        {
            WriteUInt16((UInt16)value.Length);
            Position += Encoding.GetBytes(value, Span.Slice(Position));
        }

        /// <summary>
        /// Writes a <see cref="ReadOnlySpan{Char}"/> at the current position. It has an <see cref="Int32"/> prefix
        /// determining the number of chars out of which the string consists, and no postfix.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString4(ReadOnlySpan<char> value)
        {
            WriteInt32(value.Length);
            Position += Encoding.GetBytes(value, Span.Slice(Position));
        }

        /// <summary>
        /// Writes a <see cref="ReadOnlySpan{Char}"/> at the current position. It has neither prefix nor postfix, and is
        /// stored in a buffer with the given <paramref name="byteCount"/>, padded with 0 if it is longer than the
        /// string data.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <param name="byteCount">The number of bytes to store the value in.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteStringFix(ReadOnlySpan<char> value, int byteCount)
        {
            Span.Slice(Position, byteCount).Clear();
            if (Encoding.GetBytes(value, Span.Slice(Position)) > byteCount)
                throw new ArgumentOutOfRangeException($"The string data does not fit into {byteCount} bytes.");
            Position += byteCount;
        }

        /// <summary>
        /// Writes a <see cref="ReadOnlySpan{Char}"/> at the current position. It has neither prefix nor postfix.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteStringRaw(ReadOnlySpan<char> value)
        {
            Position += Encoding.GetBytes(value, Span.Slice(Position));
        }
#endif

        /// <summary>
        /// Writes a <see cref="UInt16"/> at the current position.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt16(UInt16 value)
        {
            if (_reverseEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            MemoryMarshal.Write(Span.Slice(Position), ref value);
            Position += sizeof(UInt16);
        }

        /// <summary>
        /// Writes a <see cref="UInt32"/> at the current position.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt32(UInt32 value)
        {
            if (_reverseEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            MemoryMarshal.Write(Span.Slice(Position), ref value);
            Position += sizeof(UInt32);
        }

        /// <summary>
        /// Writes a <see cref="UInt64"/> at the current position.
        /// </summary>
        /// <param name="value">The value to be written at the current position.</param>
        /// <exception cref="ArgumentOutOfRangeException">There are less bytes available than required.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt64(UInt64 value)
        {
            if (_reverseEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            MemoryMarshal.Write(Span.Slice(Position), ref value);
            Position += sizeof(UInt64);
        }
    }
}
