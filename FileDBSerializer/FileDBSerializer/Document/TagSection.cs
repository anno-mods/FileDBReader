using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoMods.BBDom
{
    public class TagSection
    {
        public IReadOnlyDictionary<ushort, string> Tags { get => _tags; }
        public IReadOnlyDictionary<ushort, string> Attribs { get => _attribs; }

        private Dictionary<ushort, string> _tags;
        private Dictionary<ushort, string> _attribs;

        private const ushort NoneTag = 1; //0x01 0x00
        private const ushort NoneAttrib = 32768; //0x01 0x80
        private const string None = "None";

        public ushort MaxAttribID { get; set; }
        public ushort MaxTagID { get; set; }

        public TagSection()
        {
            _tags = new Dictionary<ushort, string>();
            _attribs = new Dictionary<ushort, string>();
            MaxTagID = NoneTag;
            MaxAttribID = NoneAttrib;
        }

        public TagSection(Dictionary<ushort, string> tags, Dictionary<ushort, string> attribs)
        {
            _tags = tags;
            _attribs = attribs;
            MaxTagID = (ushort)(tags.Keys.Count() > 0 ? 
                    _tags.Keys.Max() + 1 
                    : NoneTag +1);
            MaxAttribID = (ushort)(_attribs.Keys.Count() > 0 ? 
                _attribs.Keys.Max() + 1 
                : NoneAttrib + 1);
        }

        public string? GetTagName(int id)
        {
            if (id.Equals(NoneTag))
                return None;
            else return Tags.GetValueOrDefault((ushort)id);
        }

        public string? GetAttribName(int id)
        {
            if (id.Equals(NoneAttrib))
                return None;
            else return Attribs.GetValueOrDefault((ushort)id) ?? "a_" + id;
        }

        public ushort GetOrCreateTagId(string name)
        {
            if (name.Equals("None"))
                return NoneTag;

            if (!_tags.ContainsValue(name))
            {
                MaxTagID++;
                _tags.Add(MaxTagID, name);
                return MaxTagID;
            }

            return Tags.FirstOrDefault(x => x.Value == name).Key;
        }

        public ushort GetOrCreateAttribId(string name)
        {
            if (name.Equals("None"))
                return NoneAttrib;

            if (!_attribs.ContainsValue(name))
            {
                MaxAttribID++;
                _attribs.Add(MaxAttribID, name);
                return MaxAttribID;
            }

            return _attribs.FirstOrDefault(x => x.Value == name).Key;
        }
    }
}
