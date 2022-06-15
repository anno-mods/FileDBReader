using FileDBSerializing.EncodingAwareStrings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace FileDBSerializing.ObjectSerializer
{
    internal static class TypeExtensions
    {
        //sorry vera I needed to copy this ^^
        public static bool IsEnumerable(this Type type) => type != typeof(String) && (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type));

        public static bool IsStringType(this Type t) => t == typeof(String) || t.IsSubclassOf(typeof(EncodingAwareString));

        public static bool IsPrimitiveType(this Type type) => PrimitiveTypeConverter.SupportsType(type);

        public static bool IsPrimitiveOrString(this Type type) => type.IsPrimitiveType() || type.IsStringType();

        public static bool IsArray(this Type type) => type.IsArray;

        public static bool IsPrimitiveArray(this Type type) => type.IsArray && (type.GetElementType()?.IsPrimitiveType() ?? false);

        public static object? GetDefault(this Type type)
        {
            if (type.IsValueType) return Activator.CreateInstance(type);
            return null;
        }
    }

    internal static class IEnumerableExtensions
    {
        internal static Type GetContentType<T>(this IEnumerable<T> enumerable) => typeof(T);
    }

    internal static class PropertyInfoExtensions
    {
        internal static void SetArray(this PropertyInfo PropertyInfo, object? ParentObject, Array Array)
        {
            var PropertyType = PropertyInfo.PropertyType;
            PropertyInfo.SetValue(ParentObject, Convert.ChangeType(Array, PropertyType));
        }

        internal static bool HasAttribute<T>(this PropertyInfo that) where T : Attribute => that.GetCustomAttribute<T>() is not null;

        internal static Type GetNullablePropertyType(this PropertyInfo that)
        {
            Type? _nulltype;
            if ((_nulltype = Nullable.GetUnderlyingType(that.PropertyType)) is not null)
            {
                return _nulltype;
            }
            return that.PropertyType;
        }
    }
}
