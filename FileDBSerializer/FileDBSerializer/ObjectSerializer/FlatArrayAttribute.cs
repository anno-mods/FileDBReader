using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoMods.BBDom.ObjectSerializer
{
    //Set this attribute to achieve arrays without a wrapper around them
    [AttributeUsage(AttributeTargets.Property)]
    public class FlatArrayAttribute : Attribute
    {
        private bool _isFlat;

        public virtual bool IsFlatArray {
            get => _isFlat;
        }
        public FlatArrayAttribute(bool value)
        {
            _isFlat = value;
        }

        public FlatArrayAttribute()
        {
            _isFlat = true;
        }

    }
}
