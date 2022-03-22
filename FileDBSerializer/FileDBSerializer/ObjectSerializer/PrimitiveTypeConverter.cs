using System;
using System.Collections.Generic;
using System.Text;

namespace FileDBSerializing.ObjectSerializer
{
    public class PrimitiveTypeConverter
    {
        public PrimitiveTypeConverter()
        {

        }
        //----- ########################### -----//
        //      Make sure that the types are the same in both dictionaries!!!!
        //      Else this will mess with SupportsType() and lead to fuckwhat-types of errors.
        //      Future self, thank me in advance :) 
        //----- ########################### -----//

        //dictionary that maps type to functions that return byte array
        private static Dictionary<Type, Func<object, byte[]>> PrimitiveTypes = new()
        {
            { typeof(bool), s => BitConverter.GetBytes((bool)s) },
            { typeof(byte), s => new byte[] { (byte)s } },
            { typeof(sbyte), s => new byte[] { (byte)s } },
            { typeof(short), s => BitConverter.GetBytes((short)s) },
            { typeof(ushort), s => BitConverter.GetBytes((ushort)s) },
            { typeof(int), s => BitConverter.GetBytes((int)s) },
            { typeof(uint), s => BitConverter.GetBytes((uint)s) },
            { typeof(long), s => BitConverter.GetBytes((long)s) },
            { typeof(ulong), s => BitConverter.GetBytes((ulong)s) },
            { typeof(float), s => BitConverter.GetBytes((float)s) },
            { typeof(double), s => BitConverter.GetBytes((double)s) }
        };

        private static Dictionary<Type, Func<byte[], object>> PrimitiveTypesBack = new()
        {
            { typeof(bool), x => { if (x.Length != 1) throw new InvalidCastException(); return BitConverter.ToBoolean(x, 0); } },
            { typeof(byte), x => { if (x.Length != sizeof(byte)) throw new InvalidCastException(); return x[0]; } },
            { typeof(sbyte), x => { if (x.Length != sizeof(sbyte)) throw new InvalidCastException(); return (sbyte)x[0]; } },
            { typeof(short), x => { if (x.Length != sizeof(short)) throw new InvalidCastException(); return BitConverter.ToInt16(x, 0); } },
            { typeof(ushort), x => { if (x.Length != sizeof(short)) throw new InvalidCastException(); return BitConverter.ToUInt16(x, 0); } },
            { typeof(int), x => { if (x.Length != sizeof(int)) throw new InvalidCastException(); return BitConverter.ToInt32(x, 0); } },
            { typeof(uint), x => { if (x.Length != sizeof(uint)) throw new InvalidCastException(); return BitConverter.ToUInt32(x, 0); } },
            { typeof(long), x => { if (x.Length != sizeof(long)) throw new InvalidCastException(); return BitConverter.ToInt64(x, 0); } },
            { typeof(ulong), x => { if (x.Length != sizeof(ulong)) throw new InvalidCastException(); return BitConverter.ToUInt64(x, 0); } },
            { typeof(float), x => { if (x.Length != sizeof(float)) throw new InvalidCastException(); return BitConverter.ToSingle(x, 0); } },
            { typeof(double), x => { if (x.Length != sizeof(double)) throw new InvalidCastException(); return BitConverter.ToDouble(x, 0); } }
        };


        public byte[] GetBytes(object t)
        {
            if (SupportsType(t.GetType()))
                return PrimitiveTypes[t.GetType()](t);
            else
                throw new InvalidOperationException(t.GetType() + " is not a primitive type!");
        }

        public object GetObject(Type TargetType, byte[] b)
        {
            if (!SupportsType(TargetType)) throw new InvalidOperationException($"{TargetType} is not a primitive type");

            return PrimitiveTypesBack[TargetType](b);
        }

        public TargetType GetObject<TargetType>(byte[] b) 
            => (TargetType)GetObject(typeof(TargetType), b);

        public static bool SupportsType(Type t)
            => PrimitiveTypes.ContainsKey(t);
    }
}
