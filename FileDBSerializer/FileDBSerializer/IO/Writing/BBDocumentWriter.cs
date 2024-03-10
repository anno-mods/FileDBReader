using System;
using System.Diagnostics;
using System.IO;

namespace AnnoMods.BBDom.IO
{
    public class BBDocumentWriter
    {
        private IBBStructureWriter? _structureWriter;

        public BBDocumentVersion Version { get; }

        public BBDocumentWriter(BBDocumentVersion version)
        {
            Version = version;
        }

        public Stream WriteToStream(BBDocument filedb, Stream target)
        {
            if (!target.CanWrite) throw new ArgumentException("Stream needs to be writable!");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            _structureWriter = DependencyVersions.GetWriter(Version, target);
            _structureWriter.WriteNodeCollection(filedb.Roots);
            _structureWriter.WriteNodeTerminator();
            _structureWriter.WriteTagSection(filedb);
            _structureWriter.WriteMagicBytes();
            _structureWriter.Flush();

            target.Position = 0;

            stopWatch.Stop();
            Console.WriteLine("FILEDB writing took {0} ms", stopWatch.Elapsed.TotalMilliseconds);
            return target;
        }
    }
}
