using AnnoMods.BBDom;
using AnnoMods.BBDom.LookUps;
using System;

namespace AnnoMods.ObjectSerializer
{
    public enum PolymorphRootNode
    {
        Parent,
        Self
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PolymorphAttribute : Attribute
    {
        public Type TargetType { private set; get; }
        public string Path { private set; get; }

        public PolymorphRootNode Node { set; get; } = PolymorphRootNode.Self;
        public string? HexValue { set; get; } = null;

        public PolymorphAttribute(Type type, string path)
        {
            TargetType = type;
            Path = path;
        }

        public bool IsApplicable(Tag tag)
        {
            var startNode = Node == PolymorphRootNode.Self ? tag : tag.Parent;
            var el = startNode.SelectSingleNode(Path);
            if (el is null)
                return false;

            // path only check is done
            if (HexValue is null)
                return true;

            if (el is Attrib attrib)
                return HexValue == BitConverter.ToString(attrib.Content).Replace("-", "");
            return false;
        }
    }
}
