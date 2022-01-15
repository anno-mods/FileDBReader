using System;
using System.Collections.Generic;

namespace FileDBSerializing.ObjectSerializer
{
    public class PrimitiveTypeConverter
    {
        public PrimitiveTypeConverter()
        {

        }
        //dictionary that maps type to functions that return byte arrays

        private static Dictionary<Type, Func<object, byte[]>> PrimitiveTypes = new Dictionary<Type, Func<object, byte[]>>
        {
            { typeof(bool),     s => BitConverter.GetBytes((bool)s)},
            { typeof(byte),     s => new byte[] { (byte)s }},
            { typeof(sbyte),    s => new byte[] { (byte)s }},
            { typeof(short),    s => BitConverter.GetBytes((short)s) },
            { typeof(ushort),   s => BitConverter.GetBytes((ushort)s)},
            { typeof(int),      s => BitConverter.GetBytes((int)s)},
            { typeof(uint),     s => BitConverter.GetBytes((uint)s)},
            { typeof(long),     s => BitConverter.GetBytes((long)s)},
            { typeof(ulong),    s => BitConverter.GetBytes((ulong)s)},
            { typeof(float),    s => BitConverter.GetBytes((float)s)},
            { typeof(double),   s => BitConverter.GetBytes((double)s)}
        };

        public byte[] GetBytes(object t)
        {
            if (SupportsType(t.GetType()))
                return PrimitiveTypes[t.GetType()](t);
            else
                throw new InvalidOperationException(t.GetType() + " is not a primitive type!");
        }

        public static bool SupportsType(Type t)
        {
            return PrimitiveTypes.ContainsKey(t);
        }
    }
}
