using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializer.ObjectSerializer
{
    /// <summary>
    /// PropertyLocationAttribute can be added to a Class to inform that its Properties should be serialized in a specific location in relation to its base class.
    /// It can also be added to a Property to locate that specific property related to the base class' properties during serialization.
    /// When adding the Attribute to both class and property, the attribute on the property will take precedence.
    /// This Attribute has no effect if the targetet class (or class of the respective property) has no base class other than object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class PropertyLocationAttribute : Attribute
    {
        public PropertyLocationAttribute(PropertyLocationOption location)
        {
            Location = location;
        }

        public PropertyLocationOption Location { get; }
    }

    public enum PropertyLocationOption
    {
        BEFORE_PARENT,
        AFTER_PARENT
    }
}
