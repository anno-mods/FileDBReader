using FileDBSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    public enum FileDBNodeType { Tag, Attrib }

    public abstract class BBNode
    {
        public BBDocument ParentDoc;
        public Tag Parent;
        public int Bytesize = 0;
        public int ID = -1;

        public String Name { get => GetName(); }

        public FileDBNodeType NodeType;
        public abstract String GetNameWithFallback();

        public abstract String GetName();

        public IEnumerable<BBNode> Siblings => Parent is null ? ParentDoc.Roots : Parent.Children;
    }
}
