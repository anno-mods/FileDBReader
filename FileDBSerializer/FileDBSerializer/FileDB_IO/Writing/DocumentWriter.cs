using System;
using System.IO;
using System.Diagnostics;

namespace FileDBSerializing
{
    public class DocumentWriter
    {
        private IFileDBWriter StructureWriter;

        #region serialize
        /// <summary>
        /// Serializes a FileDBDocument to a Stream.
        /// </summary>
        /// <param name="filedb"></param>
        /// <param name="s">The Stream that should be serialized to.</param>
        /// <returns>the UNCLOSED Stream that contains a serialized version of the document</returns>
        public Stream WriteFileDBToStream (IFileDBDocument filedb, Stream s)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            BinaryWriter writer = new BinaryWriter(s);

            if (filedb.VERSION == FileDBDocumentVersion.Version1)
            {
                StructureWriter = new FileDBWriter_V1(writer);
            }
            else if (filedb.VERSION == FileDBDocumentVersion.Version2)
            {
                StructureWriter = new FileDBWriter_V2(writer);
            }

            StructureWriter.WriteNodeCollection(filedb.Roots);
            StructureWriter.WriteNodeTerminator(); 

            StructureWriter.RemoveNonesAndWriteTagSection(filedb.Tags);
            StructureWriter.WriteMagicBytes();

            writer.Flush();

            s.Position = 0;
            stopWatch.Stop();
            Console.WriteLine("FILEDB writing took {0} ms", stopWatch.Elapsed.TotalMilliseconds);
            return s;
        }
        #endregion
    }
}
