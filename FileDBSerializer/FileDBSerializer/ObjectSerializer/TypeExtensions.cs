using FileDBSerializer.EncodingAwareStrings;
using System;
using System.Collections;
using System.Collections.Generic;

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
            return t == typeof(String) || t.IsSubclassOf(typeof(EncodingAwareString));
        }

        public static bool IsPrimitiveType(this Type type)
        {
            return PrimitiveTypeConverter.SupportsType(type);
        }

        public static bool IsPrimitiveOrString(this Type type)
        {
            return type.IsPrimitiveType() || type.IsStringType();
        }

        public static bool IsArray(this Type type)
        {
            return type.IsArray;
        }
    }

    internal static class IEnumerableExtensions
    {
        internal static Type GetContentType<T>(this IEnumerable<T> enumerable)
        {
            return typeof(T);
        }
    }
}
