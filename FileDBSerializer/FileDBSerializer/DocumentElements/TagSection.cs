using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    public class TagSection
    {
        public IReadOnlyDictionary<ushort, String> Tags { get => _tags; }
        public IReadOnlyDictionary<ushort, String> Attribs { get => _attribs; }

        private Dictionary<ushort, String> _tags;
        private Dictionary<ushort, String> _attribs;

        private const ushort NoneTag = 1; //0x01 0x00
        private const ushort NoneAttrib = 32768; //0x01 0x80
        private const String None = "None";

        public ushort MaxAttribID { get; set; }
        public ushort MaxTagID { get; set; }

        public TagSection()
        {
            _tags = new Dictionary<ushort, String>();
            _attribs = new Dictionary<ushort, String>();            
            MaxTagID = NoneTag;
            MaxAttribID = NoneAttrib;
        }

        public TagSection(Dictionary<ushort, String> tags, Dictionary<ushort, String> attribs)
        {
            _tags = tags;
            _attribs = attribs;
            MaxTagID = (ushort) (_tags.Keys.Max() + 1);
            MaxAttribID = (ushort) (_attribs.Keys.Max() + 1);
        }

        public String? GetTagName(int id)
        {
            if (id.Equals(NoneTag))
                return None;
            else return Tags.GetValueOrDefault((ushort)id);
        }

        public String? GetAttribName(int id)
        {
            if (id.Equals(NoneAttrib))
                return None;
            else return Attribs.GetValueOrDefault((ushort)id) ?? "a_" + id;
        }

        public ushort GetOrCreateTagId(String name)
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

        public ushort GetOrCreateAttribId(String name)
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
