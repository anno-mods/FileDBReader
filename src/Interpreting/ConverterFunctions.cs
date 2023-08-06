using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace FileDBReader
{
    public static class ConverterFunctions
    {
        #region FunctionDictionaries
        /// <summary>
        /// Conversion Rules: Hex String to their Number Representation.
        /// </summary>
        public static Dictionary<Type, Func<string, Encoding, String>> ConversionRulesImport = new Dictionary<Type, Func<string, Encoding, String>>
            {
                { typeof(bool),   (s, Encoding) => PrimitiveTypeConverter.GetObject<bool>(HexHelper.ToBytes(s)).ToString()},
                { typeof(byte),   (s, Encoding) => PrimitiveTypeConverter.GetObject<byte>(HexHelper.ToBytes(s)).ToString()},
                { typeof(sbyte),  (s, Encoding) => PrimitiveTypeConverter.GetObject<sbyte>(HexHelper.ToBytes(s)).ToString() },
                { typeof(short),  (s, Encoding) => PrimitiveTypeConverter.GetObject<short>(HexHelper.ToBytes(s)).ToString() },
                { typeof(ushort), (s, Encoding) => PrimitiveTypeConverter.GetObject<ushort>(HexHelper.ToBytes(s)).ToString() },
                { typeof(int),    (s, Encoding) => PrimitiveTypeConverter.GetObject<int>(HexHelper.ToBytes(s)).ToString() },
                { typeof(uint),   (s, Encoding) => PrimitiveTypeConverter.GetObject<uint>(HexHelper.ToBytes(s)).ToString() },
                { typeof(long),   (s, Encoding) => PrimitiveTypeConverter.GetObject<long>(HexHelper.ToBytes(s)).ToString() },
                { typeof(ulong),  (s, Encoding) => PrimitiveTypeConverter.GetObject<ulong>(HexHelper.ToBytes(s)).ToString() },
                { typeof(float),  (s, Encoding) => PrimitiveTypeConverter.GetObject<float>(HexHelper.ToBytes(s)).ToString() },
                { typeof(double), (s, Encoding) => PrimitiveTypeConverter.GetObject<double>(HexHelper.ToBytes(s)).ToString() },
                { typeof(String), (s, Encoding) => ToString(s, Encoding)}
            };

        /// <summary>
        /// Conversion Rules: Number Strings to their Hex Representation.
        /// </summary>
        public static Dictionary<Type, Func<String, Encoding, byte[]>> ConversionRulesExport = new Dictionary<Type, Func<String, Encoding, byte[]>>
            {
                { typeof(bool),     (s, Encoding)   => PrimitiveTypeConverter.GetBytes(bool.Parse(s))},
                { typeof(byte),     (s, Encoding)   => PrimitiveTypeConverter.GetBytes(byte.Parse(s))},
                { typeof(sbyte),    (s, Encoding)   => PrimitiveTypeConverter.GetBytes(sbyte.Parse(s))},
                { typeof(short),    (s, Encoding)   => PrimitiveTypeConverter.GetBytes(short.Parse(s))},
                { typeof(ushort),   (s, Encoding)   => PrimitiveTypeConverter.GetBytes(ushort.Parse(s))},
                { typeof(int),      (s, Encoding)   => PrimitiveTypeConverter.GetBytes(int.Parse(s))},
                { typeof(uint),     (s, Encoding)   => PrimitiveTypeConverter.GetBytes(uint.Parse(s))},
                { typeof(long),     (s, Encoding)   => PrimitiveTypeConverter.GetBytes(long.Parse(s))},
                { typeof(ulong),    (s, Encoding)   => PrimitiveTypeConverter.GetBytes(ulong.Parse(s))},
                { typeof(float),    (s, Encoding)   => 
                    {
                        var normalized = s.Replace(",", ".");
                        return PrimitiveTypeConverter.GetBytes(float.Parse(s, CultureInfo.InvariantCulture));
                    }},
                { typeof(double),   (s, Encoding)   => 
                    {
                        var normalized = s.Replace(",", ".");
                        return PrimitiveTypeConverter.GetBytes(double.Parse(s, CultureInfo.InvariantCulture));
                    }},
                { typeof(String),   (s, Encoding)   => Encoding.GetBytes(s)}
            };

        public static Dictionary<Type, Func<string, object>> ConversionRulesToObject = new Dictionary<Type, Func<string, object>>
            {
                { typeof(bool),   s => PrimitiveTypeConverter.GetObject<bool>(HexHelper.ToBytes(s))},
                { typeof(byte),   s => PrimitiveTypeConverter.GetObject<byte>(HexHelper.ToBytes(s))},
                { typeof(sbyte),  s => PrimitiveTypeConverter.GetObject<sbyte>(HexHelper.ToBytes(s))},
                { typeof(short),  s => PrimitiveTypeConverter.GetObject<short>(HexHelper.ToBytes(s)) },
                { typeof(ushort), s => PrimitiveTypeConverter.GetObject<ushort>(HexHelper.ToBytes(s)) },
                { typeof(int),    s => PrimitiveTypeConverter.GetObject<int>(HexHelper.ToBytes(s))},
                { typeof(uint),   s => PrimitiveTypeConverter.GetObject<uint>(HexHelper.ToBytes(s))},
                { typeof(long),   s => PrimitiveTypeConverter.GetObject<long>(HexHelper.ToBytes(s))},
                { typeof(ulong),  s => PrimitiveTypeConverter.GetObject<ulong>(HexHelper.ToBytes(s))},
                { typeof(double), s => PrimitiveTypeConverter.GetObject<double>(HexHelper.ToBytes(s))},
                { typeof(float),  s => PrimitiveTypeConverter.GetObject<float>(HexHelper.ToBytes(s))},
                { typeof(String), s => ToString(s, new UnicodeEncoding())}
            };
        
        public static Dictionary<Type, Func<String, String>> ListFunctionsInterpret = new Dictionary<Type, Func<String, String>>
            {
                { typeof(bool),   s => Join<bool>(s)},
                { typeof(byte),   s => Join<byte>(s) },
                { typeof(sbyte),  s => Join<sbyte>(s) },
                { typeof(short),  s => Join<short>(s) },
                { typeof(ushort), s => Join<ushort>(s) },
                { typeof(int),    s => Join<int>(s) },
                { typeof(uint),   s => Join<uint>(s) },
                { typeof(long),   s => Join<long>(s) },
                { typeof(ulong),  s => Join<ulong>(s) },
                { typeof(float),  s => Join<float>(s) }
            };
        #endregion


        //using String.Join for performance optimization over for loops.
        public static String Join<T>(String BinaryData) where T : struct
        {
            var span = toSpan<T>(BinaryData).ToArray();
            return String.Join<T>(" ", span);
        }

        //Converts a Hex String to a span of bytes
        public static Span<T> toSpan<T>(String BinHex) where T : struct
        {
            int size = Marshal.SizeOf(default(T)) * 2;
            var bytes = new T[BinHex.Length / size];
            Type t = typeof(T);
            for (var i = 0; i < bytes.Length; i++)
            {
                try
                {
                    bytes[i] = (T)ConverterFunctions.ConversionRulesToObject[t](BinHex.Substring(i * size, size));
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not convert to Span. Hex String not in correct format: {0}", BinHex);
                }
            }
            return bytes.AsSpan();
        }

        public static string ToString(string hexString, Encoding encoding)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                try
                {
                    bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }
                catch (Exception)
                {
                    Console.WriteLine("[VALUE CONVERSION]: Hex String not in correct format: {0}", hexString);
                }
            }
            return encoding.GetString(bytes);
        }
    }
}
