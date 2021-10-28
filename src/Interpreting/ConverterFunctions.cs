using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileDBReader
{
    public static class ConverterFunctions
    {
        public static Dictionary<Type, Func<string, Encoding, String>> ConversionRulesImport = new Dictionary<Type, Func<string, Encoding, String>>
            {
                { typeof(bool),   (s, Encoding) => HexHelper.ToBool(s).ToString()},
                { typeof(byte),   (s, Encoding) => byte.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(sbyte),  (s, Encoding) => sbyte.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(short),  (s, Encoding) => short.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(ushort), (s, Encoding) => ushort.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(int),    (s, Encoding) => int.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(uint),   (s, Encoding) => uint.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(long),   (s, Encoding) => long.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(ulong),  (s, Encoding) => ulong.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(float),  (s, Encoding) => HexHelper.ToFloat(HexHelper.flip(s)).ToString() },
                { typeof(double), (s, Encoding) => HexHelper.ToDouble(HexHelper.flip(s)).ToString() },
                { typeof(String), (s, Encoding) => HexHelper.FromHexString(s, Encoding) }
            };

        public static Dictionary<Type, Func<String, Encoding, byte[]>> ConversionRulesExport = new Dictionary<Type, Func<String, Encoding, byte[]>>
            {
                { typeof(bool),     (s, Encoding)   => BitConverter.GetBytes(bool.Parse(s))},
                { typeof(byte),     (s, Encoding)   => new byte[] { byte.Parse(s) }},
                { typeof(sbyte),    (s, Encoding)   => new byte[] { (byte)sbyte.Parse(s) }},
                { typeof(short),    (s, Encoding)   => BitConverter.GetBytes(short.Parse(s))},
                { typeof(ushort),   (s, Encoding)   => BitConverter.GetBytes(ushort.Parse(s))},
                { typeof(int),      (s, Encoding)   => BitConverter.GetBytes(int.Parse(s))},
                { typeof(uint),     (s, Encoding)   => BitConverter.GetBytes(uint.Parse(s))},
                { typeof(long),     (s, Encoding)   => BitConverter.GetBytes(long.Parse(s))},
                { typeof(ulong),    (s, Encoding)   => BitConverter.GetBytes(ulong.Parse(s))},
                { typeof(float),    (s, Encoding)   => BitConverter.GetBytes(float.Parse(s))},
                { typeof(double),   (s, Encoding)   => BitConverter.GetBytes(double.Parse(s))},
                { typeof(String),   (s, Encoding)   => Encoding.GetBytes(s)}
            };

        public static Dictionary<Type, Func<string, object>> ConversionRulesToObject = new Dictionary<Type, Func<string, object>>
            {
                { typeof(bool),   s => HexHelper.ToBool(s).ToString()},
                { typeof(byte),   s => byte.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier) },
                { typeof(sbyte),  s => sbyte.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier) },
                { typeof(short),  s => short.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier) },
                { typeof(ushort), s => ushort.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier) },
                { typeof(int),    s => int.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier) },
                { typeof(uint),   s => uint.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier) },
                { typeof(long),   s => long.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier) },
                { typeof(ulong),  s => ulong.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier) },
                { typeof(double), s => HexHelper.ToDouble(HexHelper.flip(s)) },
                { typeof(float),  s => HexHelper.ToFloat(HexHelper.flip(s)) },
                { typeof(String), s => HexHelper.FromHexString(s, new UnicodeEncoding()) }
            };
        public static Dictionary<Type, Func<String, String>> ListFunctionsInterpret = new Dictionary<Type, Func<String, String>>
            {
                { typeof(bool),   s => HexHelper.Join<bool>(s)},
                { typeof(byte),   s => HexHelper.Join<byte>(s) },
                { typeof(sbyte),  s => HexHelper.Join<sbyte>(s) },
                { typeof(short),  s => HexHelper.Join<short>(s) },
                { typeof(ushort), s => HexHelper.Join<ushort>(s) },
                { typeof(int),    s => HexHelper.Join<int>(s) },
                { typeof(uint),   s => HexHelper.Join<uint>(s) },
                { typeof(long),   s => HexHelper.Join<long>(s) },
                { typeof(ulong),  s => HexHelper.Join<ulong>(s) },
                { typeof(float),  s => HexHelper.Join<float>(s) }
            };
    }
}
