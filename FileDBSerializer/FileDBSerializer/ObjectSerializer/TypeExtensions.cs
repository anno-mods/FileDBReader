using FileDBSerializer.ObjectSerializer;
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

        internal static String GetNameWithRenaming(this PropertyInfo property)
        {
            if (property.HasAttribute<RenamePropertyAttribute>())
            {
                var attrib = property.GetCustomAttribute<RenamePropertyAttribute>();
                if(attrib is not null) 
                    return attrib.RenameTo;
            }
            return property.Name;
        }

        internal static PropertyInfo? GetPropertyWithRenaming(this Type type, String name)
        {
            var properties = type.GetProperties();

            //try to get the renamed first 
            if (properties.Any(x => x.HasAttribute<RenamePropertyAttribute>()))
            {
                var withRename = properties.Where(x => x.HasAttribute<RenamePropertyAttribute>())
                    .Where(x => (x.GetCustomAttribute(typeof(RenamePropertyAttribute)) as RenamePropertyAttribute)?.RenameTo.Equals(name) ?? false);
                if (withRename.Count() > 1)
                    throw new Exception($"Multiple properties with the same name exist in: {type.Name}");
                var property = withRename.FirstOrDefault();
                
                if(property is not null) return property;
            }
            //if no renamed exist, just do it default way.
            return type.GetProperty(name);
        }

        /// <summary>
        /// Get Properties of a class with its base class properties respecting the PropertyLocationAttribute if present.
        /// </summary>
        /// <param name="type">The targeted Type.</param>
        /// <returns>An IEnumerable&lt;PropertyInfo&gt; containing the ordered PropertyInfos.</returns>
        public static IEnumerable<PropertyInfo> GetPropertiesWithOrder(this Type type)
        {
            List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
            int nThBefore = 0;

            void PlacePropertyInfo(PropertyInfo item, PropertyLocationOption location)
            {
                if(location == PropertyLocationOption.BEFORE_PARENT)
                {
                    propertyInfos.Insert(nThBefore, item);
                    nThBefore++;
                }
                else
                {
                    propertyInfos.Add(item);
                }
            }


            var baseType = type.BaseType;
            if(baseType is not null && baseType != typeof(object))
            {
                propertyInfos.AddRange(baseType.GetPropertiesWithOrder());
            }

            var currentProperties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            var orderedProperties = currentProperties.OrderBy(x => x.MetadataToken);

            foreach(PropertyInfo info in orderedProperties)
            {
                if(info.GetCustomAttribute<PropertyLocationAttribute>() is PropertyLocationAttribute propertyAttribute && propertyAttribute is not null)
                {
                    PlacePropertyInfo(info, propertyAttribute.Location);
                }
                else if(type.GetCustomAttribute<PropertyLocationAttribute>() is PropertyLocationAttribute typeAttribute && typeAttribute is not null)
                {
                    PlacePropertyInfo(info, typeAttribute.Location);
                }
                else
                {
                    //Default to BEFORE_PARENT as that is the default language behaviour
                    PlacePropertyInfo(info, PropertyLocationOption.BEFORE_PARENT);
                }
            }

            return propertyInfos;
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
