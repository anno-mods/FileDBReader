using AnnoMods.BBDom;
using AnnoMods.BBDom.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoMods.BBDom.IO
{
    public class DependencyVersions
    {
        private static Dictionary<BBDocumentVersion, Func<Stream, IBBStructureParser>> Parsers = new()
        {
            { BBDocumentVersion.V1, (s) => new BBStructureParser_V1(s) },
            { BBDocumentVersion.V2, (s) => new BBStructureParser_V2(s) },
            { BBDocumentVersion.V3, (s) => new BBStructureParser_V2(s) }
        };

        private static Dictionary<BBDocumentVersion, Func<Stream, IBBStructureWriter>> Writers = new()
        {
            { BBDocumentVersion.V1, (s) => new BBStructureWriter_V1(s) },
            { BBDocumentVersion.V2, (s) => new BBStructureWriter_V2(s) },
            { BBDocumentVersion.V3, (s) => new BBStructureWriter(s) }
        };

        public static BBDocument GetDocument(BBDocumentVersion version)
        {
            Versioning.EnsureVersion(version);
            return new BBDocument();
        }

        public static IBBStructureParser GetParser(BBDocumentVersion version, Stream source)
        {
            Versioning.EnsureVersion(version);
            return Parsers[version].Invoke(source);
        }

        public static IBBStructureWriter GetWriter(BBDocumentVersion version, Stream target)
        {
            Versioning.EnsureVersion(version);
            return Writers[version].Invoke(target);
        }
    }
}
