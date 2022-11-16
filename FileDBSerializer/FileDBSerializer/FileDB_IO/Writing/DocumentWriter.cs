using System;
using System.IO;
using System.Diagnostics;

namespace FileDBSerializing
{
    public class DocumentWriter
    {
        private IFileDBWriter? StructureWriter;

        #region serialize
        /// <summary>
        /// Serializes a FileDBDocument to a Stream.
        /// </summary>
        /// <param name="filedb"></param>
        /// <param name="s">The Stream that should be serialized to.</param>
        /// <returns>the UNCLOSED Stream that contains a serialized version of the document</returns>
        public Stream WriteFileDBToStream (IFileDBDocument filedb, Stream target)
        {
            if (!target.CanWrite) throw new ArgumentException("Stream needs to be writable!");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            StructureWriter = DependencyVersions.GetWriter(filedb.VERSION, target);
            StructureWriter.WriteNodeCollection(filedb.Roots);
            StructureWriter.WriteNodeTerminator(); 
            StructureWriter.RemoveNonesAndWriteTagSection(filedb);
            StructureWriter.WriteMagicBytes();
            StructureWriter.Flush();

            target.Position = 0;

            stopWatch.Stop();
            Console.WriteLine("FILEDB writing took {0} ms", stopWatch.Elapsed.TotalMilliseconds);
            return target;
        }
        #endregion
    }
}
