using System.IO;

namespace AnnoMods.BBDom.IO
{
    internal class BBStructureWriter : BBStructureWriter_V2
    {
        public BBStructureWriter(Stream s) : base(s)
        {

        }

        public override void WriteMagicBytes()
        {
            Writer!.Write(Versioning.GetMagicBytes(BBDocumentVersion.V3));
        }

        public override void WriteNodeCountSection(int nodeCount)
        {
            Writer!.Write(0);
            Writer!.Write(nodeCount);
        }

        public override void WriteTagSection(BBDocument forDocument)
        {
            TagSection tagSection = forDocument.TagSection;

            (int tagOffset, int attribOffset) = WriteTagsAndAttribs(tagSection);

            WriteNodeCountSection(forDocument.CountNodes());
            WriteTagOffsets(tagOffset, attribOffset);
        }
    }
}
