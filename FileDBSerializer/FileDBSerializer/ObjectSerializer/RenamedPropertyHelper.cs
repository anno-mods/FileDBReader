using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AnnoMods.BBDom.ObjectSerializer
{
    public class RenamedPropertyHelper
    {
        private ConcurrentDictionary<Type, bool> _verifiedCache;

        public RenamedPropertyHelper()
        {
            _verifiedCache = new();
        }

        public bool CheckTypeValidity(Type t)
        {
            //try to get a cached result first.
            if (_verifiedCache.TryGetValue(t, out var result))
                return result;

            List<String> PropertyNamesSoFar = new();
            PropertyInfo[] PropertiesOfType = t.GetProperties();

            bool valid = true;
            for (int i = 0; i < PropertiesOfType.Length && valid; i++)
            {
                var prop = PropertiesOfType[i];
                var name = GetNameWithRenaming(prop);
                if (PropertyNamesSoFar.Any(x => x.Equals(name)))
                    valid = false;
                PropertyNamesSoFar.Add(name);
            }
            _verifiedCache.TryAdd(t, valid);
            return valid;
        }

        public String GetNameWithRenaming(PropertyInfo property)
        {
            if (property.HasAttribute<RenamePropertyAttribute>())
            {
                var attrib = property.GetCustomAttribute<RenamePropertyAttribute>();
                if (attrib is not null)
                    return attrib.RenameTo;
            }
            return property.Name;
        }

        public PropertyInfo? GetPropertyWithRenaming(Type type, String name)
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

                if (property is not null) return property;
            }
            //if no renamed exist, just do it default way.
            return type.GetProperty(name);
        }
    }
}
