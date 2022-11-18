using FileDBSerializing.LookUps;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FileDBSerializing
{
    [DebuggerDisplay("[FileDB_Tag: ID = {ID}, Name = {Name}, ChildCount = {ChildCount}]")]
    public class Tag : FileDBNode
    {
        public List<FileDBNode> Children = new List<FileDBNode>();
        //we are only allowed to get bytesize because it is needed when writing. Otherwise, bytesize for tags is always 0!
        public int ChildCount
        {
            get
            {
                return Children.Count;
            }
        }

        new int Bytesize { get; }

        public Tag()
        {
            Bytesize = 0;
            NodeType = FileDBNodeType.Tag;
        }

        public void AddChild(FileDBNode node)
        {
            Children.Add(node);
        }

        public void AddChildren(IEnumerable<FileDBNode> nodes)
        {
            foreach (var _ in nodes)
            {
                AddChild(_);
            }
        }

        public override String GetID()
        {
            if (ParentDoc.Tags.Tags.TryGetValue((ushort)ID, out string value))
                return value;
            else return "t_" + ID;
        }

        public override string GetName()
        {
            return ParentDoc.Tags.Tags[(ushort)ID];
        }

        public int GetChildCountRecursive()
        {
            int count = ChildCount;

            foreach (FileDBNode child in Children)
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
