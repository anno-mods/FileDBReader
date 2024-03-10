using System.Collections.Generic;

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
