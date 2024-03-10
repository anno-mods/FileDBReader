using AnnoMods.BBDom.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace AnnoMods.BBDom
{
    public class BBDocument
    {
        public List<BBNode> Roots { get; set; }
        public TagSection TagSection { get; set; }
        public int ElementCount { get => Roots.Count; }
        public ushort MaxTagID { get => TagSection.MaxTagID; }
        public ushort MaxAttribID { get => TagSection.MaxAttribID; }

        public BBDocument()
        {
            Roots = new List<BBNode>();
            TagSection = new TagSection();
        }

        private void ThrowIfNodeNotInDocument(BBNode node)
        {
            if (node.ParentDoc != this)
                throw new InvalidOperationException($"Node {node} is not Part of this Document. use BBDocument.ImportNode to create the node instead");
        }

        public void AddRoot(BBNode node)
        {
            ThrowIfNodeNotInDocument(node);
            Roots.Add(node);
        }

        public static BBDocument LoadStream(Stream s)
        {
            var version = VersionDetector.GetCompressionVersion(s);
            BBDocumentParser parser = new BBDocumentParser(version);
            return parser.LoadBBDocument(s);
        }

        /// <summary>
        /// Writes a FileDBDocument to a Stream.
        /// </summary>
        /// <param name="version">The document version that should be used for writing</param>
        /// <param name="s">The Stream that should be written to.</param>
        public void WriteToStream(Stream s, BBDocumentVersion version = BBDocumentVersion.V3)
        {
            BBDocumentWriter writer = new BBDocumentWriter(version);
            writer.WriteToStream(this, s);
        }


        public int CountNodes()
        {
            int count = 1 + Roots.Count; //<Content> node + all roots

            foreach (var node in Roots)
            {
                if (node is Tag t)
                {
                    count += t.GetChildCountRecursive();
                }
            }

            return count;
        }
    }
}
