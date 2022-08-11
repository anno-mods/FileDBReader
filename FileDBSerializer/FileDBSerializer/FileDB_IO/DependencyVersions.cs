using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    public class DependencyVersions
    {
        private static Dictionary<FileDBDocumentVersion, Func<IFileDBDocument>> Documents = new()
        {
            { FileDBDocumentVersion.Version1, () => new FileDBDocument_V1() },
            { FileDBDocumentVersion.Version2, () => new FileDBDocument_V2() },
            { FileDBDocumentVersion.Version3, () => new FileDBDocument_V3() }
        };

        private static Dictionary<FileDBDocumentVersion, Func<Stream, IFileDBParser>> Parsers = new()
        {
            { FileDBDocumentVersion.Version1, (s) => new FileDBParser_V1(s) },
            { FileDBDocumentVersion.Version2, (s) => new FileDBParser_V2(s) },
            { FileDBDocumentVersion.Version3, (s) => new FileDBParser_V2(s) }
        };

        private static Dictionary<FileDBDocumentVersion, Func<Stream, IFileDBWriter>> Writers = new()
        {
            { FileDBDocumentVersion.Version1, (s) => new FileDBWriter_V1(s) },
            { FileDBDocumentVersion.Version2, (s) => new FileDBWriter_V2(s) },
            { FileDBDocumentVersion.Version3, (s) => new FileDBWriter_V3(s) }
        };

        public static IFileDBDocument GetDocument(FileDBDocumentVersion version)
        {
            Versioning.EnsureVersion(version);
            return Documents[version].Invoke();
        }

        public static IFileDBParser GetParser(FileDBDocumentVersion version, Stream source)
        {
            Versioning.EnsureVersion(version);
            return Parsers[version].Invoke(source);
        }

        public static IFileDBWriter GetWriter(FileDBDocumentVersion version, Stream target)
        {
            Versioning.EnsureVersion(version);
            return Writers[version].Invoke(target);
        }
    }
}
