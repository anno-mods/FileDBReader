using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoMods.BBDom
{
    public enum BBNodeType { Tag, Attrib }

    public abstract class BBNode
    {
        public BBDocument ParentDoc;
        public Tag Parent;
        public int Bytesize = 0;
        public int ID = -1;

        public string Name { get => GetName(); }

        public BBNodeType NodeType;
        public abstract string GetNameWithFallback();

        public abstract string GetName();

        public IEnumerable<BBNode> Siblings => Parent is null ? ParentDoc.Roots : Parent.Children;
    }
}
