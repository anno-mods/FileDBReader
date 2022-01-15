using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace FileDBSerializing

{
    public class FileDBSerializer
    {
        /*
         * ///------------------################----------------------///
         * This deserializer holds an Instance of an object that must implement IFileDBWriter
         * with each call of Serialize, this variable is newly initialized with a fresh IFileDBWriter implementation.
         * 
         * Modifying this variable outside of the intended serializing functions will break the entire serializer. 
         * ///------------------################----------------------///
         */

        private IFileDBWriter StructureWriter;

        #region serialize
        /// <summary>
        /// Serializes a FileDBDocument to a Stream.
        /// </summary>
        /// <param name="filedb"></param>
        /// <param name="s">The Stream that should be serialized to.</param>
        /// <returns>the UNCLOSED Stream that contains a serialized version of the document</returns>
        public Stream Serialize (IFileDBDocument filedb, Stream s)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            BinaryWriter writer = new BinaryWriter(s);
            if (filedb.VERSION == 1)
            {
                StructureWriter = new FileDBWriter_V1(writer);
            }
            else if (filedb.VERSION == 2)
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
            Console.WriteLine("FILEDB Serialization took {0} ms", stopWatch.Elapsed.TotalMilliseconds);
            return s;
        }
        #endregion
    }
}
