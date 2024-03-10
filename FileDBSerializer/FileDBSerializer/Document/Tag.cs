using AnnoMods.BBDom.IO;
using AnnoMods.BBDom.LookUps;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AnnoMods.BBDom
{
    [DebuggerDisplay("[BB_Tag: ID = {ID}, Name = {Name}, ChildCount = {ChildCount}]")]
    public class Tag : BBNode
    {
        public List<BBNode> Children = new List<BBNode>();
        //we are only allowed to get bytesize because it is needed when writing. Otherwise, bytesize for tags is always 0!
        public int ChildCount
        {
            get
            {
                return Children.Count;
            }
        }

        new int Bytesize { get; }

        internal Tag()
        {
            Bytesize = 0;
            NodeType = BBNodeType.Tag;
        }

        #region AddChildren

        public void AddChild(BBNode node)
        {
            Children.Add(node);
        }

        public void AddChildren(IEnumerable<BBNode> nodes)
        {
            foreach (var _ in nodes)
            {
                AddChild(_);
            }
        }

        public void AddChildren(params BBNode[] nodes) => AddChildren(nodes as IEnumerable<BBNode>);

        #endregion

        public override string GetNameWithFallback() => ParentDoc.TagSection.GetTagName(ID) ?? "t_" + ID;

        public override string GetName() => ParentDoc.TagSection.GetTagName(ID)
            ?? throw new InvalidBBException($"ID {ID} does not correspond to a Name in this Documents Tags Section.");

        public int GetChildCountRecursive()
        {
            int count = ChildCount;

            foreach (BBNode child in Children)
            {
                if (child is Tag t)
                {
                    count += t.GetChildCountRecursive();
                }
            }

            return count;
        }

    }
}
