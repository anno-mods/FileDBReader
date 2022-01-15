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

        public FileDBNodeType NodeType;
        public abstract String GetID();

        public abstract String GetName();
    }
}
