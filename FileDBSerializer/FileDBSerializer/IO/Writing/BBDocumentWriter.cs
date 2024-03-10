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

        public Stream WriteToStream(BBDocument BB, Stream target)
        {
            if (!target.CanWrite) throw new ArgumentException("Stream needs to be writable!");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            _structureWriter = DependencyVersions.GetWriter(Version, target);
            _structureWriter.WriteNodeCollection(BB.Roots);
            _structureWriter.WriteNodeTerminator();
            _structureWriter.WriteTagSection(BB);
            _structureWriter.WriteMagicBytes();
            _structureWriter.Flush();

            target.Position = 0;

            stopWatch.Stop();
            Console.WriteLine("BB Document written in {0} ms", stopWatch.Elapsed.TotalMilliseconds);
            return target;
        }
    }
}
