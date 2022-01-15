using System;
using System.Collections;

namespace FileDBSerializing.ObjectSerializer
{
    internal static class TypeExtensions
    {
        public static bool IsEnumerable(this Type type)
        {
            return type != typeof(String) && (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type));
        }

        public static bool IsStringType(this Type t)
        {
            return t == typeof(String);
        }

        public static bool IsPrimitiveType(this Type type)
        {
            return PrimitiveTypeConverter.SupportsType(type);
        }

        public static bool IsArray(this Type type)
        {
            return type.IsArray;
        }
    }
}
