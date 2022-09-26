using FileDBSerializing.EncodingAwareStrings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FileDBSerializing.ObjectSerializer
{
    internal static class TypeExtensions
    {
        //sorry vera I needed to copy this ^^
        public static bool IsEnumerable(this Type type) => type != typeof(String) && (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type));

        public static bool IsStringType(this Type t) => t == typeof(String) || t.IsSubclassOf(typeof(EncodingAwareString));

        public static bool IsEncodingAwareString(this Type t) => t.IsSubclassOf(typeof(EncodingAwareString));

        public static bool IsPrimitiveType(this Type type) => PrimitiveTypeConverter.SupportsType(type);

        public static bool IsPrimitiveOrString(this Type type) => type.IsPrimitiveType() || type.IsStringType();

        public static bool IsArray(this Type type) => type.IsArray;

        public static bool IsReference(this Type type) => !type.IsValueType;

        public static bool IsPrimitiveListType(this Type type) => type.IsArray && (type.GetElementType()?.IsPrimitiveType() ?? false);

        public static bool IsTuple(this Type type) => type.GetInterfaces().Contains(typeof(ITuple));

        public static bool IsList(this Type type) => type.GetInterfaces().Contains(typeof(IList));

        public static object? GetDefault(this Type type)
        {
            if (type.IsValueType) return Activator.CreateInstance(type);
            return null;
        }

        internal static Type GetNullableType(this Type type)
        {
            Type? _nulltype;
            if ((_nulltype = Nullable.GetUnderlyingType(type)) is not null)
            {
                return _nulltype;
            }
            return type;
        }

        internal static PropertyInfo? GetPropertyWithRenaming(this Type type, String name)
        {
            return type.GetProperty(name);
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
            return that.PropertyType.GetNullableType(); ;
        }
    }

    internal static class FileDBNodeExtensions
    {
        internal static IEnumerable<FileDBNode> AsEnumerable(this FileDBNode node)
        {
            return new FileDBNode[] { node };
        }
    }

    internal static class AttributeEnumerableExtensions
    {
        internal static bool ContainsAttribute<T>(this IEnumerable<Attribute> attribCollection) where T : Attribute 
        {
            return attribCollection.Any((attr) => attr.GetType() == typeof(FlatArrayAttribute));
        }
    }
}
