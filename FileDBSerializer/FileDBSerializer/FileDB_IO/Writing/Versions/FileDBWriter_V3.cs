using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    internal class FileDBWriter_V3 : FileDBWriter_V2
    {
        public FileDBWriter_V3(Stream s) : base(s)
        { 
        
        }

        public override void WriteMagicBytes()
        {
            Writer!.Write(Versioning.GetMagicBytes(FileDBDocumentVersion.Version3));
        }

        public override void WriteNodeCountSection(int nodeCount)
        {
            Writer!.Write(0);
            Writer!.Write(nodeCount);
        }

        public override void RemoveNonesAndWriteTagSection(IFileDBDocument forDocument)
        {
            TagSection tagSection = forDocument.Tags;

            tagSection.Tags.Remove(1);
            tagSection.Attribs.Remove(32768);

            (int tagOffset, int attribOffset) = this.WriteTagSection(tagSection);

            tagSection.Tags.Add(1, "None");
            tagSection.Attribs.Add(32768, "None");

            this.WriteNodeCountSection(forDocument.CountNodes());
            this.WriteTagOffsets(tagOffset, attribOffset);
        }
    }
}
