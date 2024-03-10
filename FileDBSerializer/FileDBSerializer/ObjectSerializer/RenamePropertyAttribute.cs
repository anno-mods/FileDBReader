using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoMods.BBDom.ObjectSerializer
{
    /// <summary>
    /// Set this attribute to rename the property in serialized FileDB.
    /// Useful for items named eg. "default", which is a C# keyword and can't be used as Property name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RenamePropertyAttribute : Attribute
    {
        public RenamePropertyAttribute(string renameTo)
        {
            RenameTo = renameTo;
        }

        public string RenameTo { get; }
    }
}
