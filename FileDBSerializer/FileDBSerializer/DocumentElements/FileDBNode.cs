using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    public enum FileDBNodeType { Tag, Attrib }

    public abstract class FileDBNode
    {
        public IFileDBDocument ParentDoc = null;
        public Tag Parent = null;
        public int Bytesize = 0;
        public int ID = -1;

        public String Name { get => GetName(); }

        public FileDBNodeType NodeType;
        public abstract String GetID();

        public abstract String GetName();

        public IEnumerable<FileDBNode> Siblings => Parent is null ? ParentDoc.Roots : Parent.Children;
    }
}
