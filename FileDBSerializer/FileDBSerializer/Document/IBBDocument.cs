using FileDBSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoMods.BBDom
{
    public interface IBBDocument
    {
        public List<BBNode> Roots { get; set; }
        public TagSection TagSection { get; set; }
        public int ElementCount { get; }
        public ushort MaxAttribID { get; }
        public ushort MaxTagID { get; }
    }
}
