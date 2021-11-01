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
        private BinaryWriter writer;

        #region serialize
        public MemoryStream Serialize(FileDBDocument filedb)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            using (MemoryStream ms = new MemoryStream())
            {
                writer = new BinaryWriter(ms);

                //actual code
                VERSION2_SerializeCollection(filedb.Roots);
                
                //(todo) serialize tag section

                stopWatch.Stop();
                Console.WriteLine("FILEDB Serialization took: " + stopWatch.Elapsed.TotalMilliseconds);
                
                return ms;
            }
        }

        #endregion

        #region COMPRESSION_VERSION_2_SUBFUNCTIONS

        private void VERSION2_SerializeNode(FileDBNode n)
        {
            writer.Write(n.Bytesize);
            writer.Write(n.ID);

            if (n is Tag)
                VERSION2_SerializeTag((Tag)n);
            else if (n is Attrib)
                VERSION2_SerializeAttrib((Attrib)n);
            writer.Flush();
        }

        private void VERSION2_SerializeTag(Tag t)
        {
            VERSION2_SerializeCollection(t.Children);
        }

        private void VERSION2_SerializeCollection(IEnumerable<FileDBNode> coll)
        {
            foreach (FileDBNode n in coll)
            {
                VERSION2_SerializeNode(n);
            }
            writer.Write((Int64)0);
        }

        private void VERSION2_SerializeAttrib(Attrib a)
        {
            writer.Write(FileDBDocument_V2.GetBytesInBlocks(a.Content, a.Bytesize));
        }

        #endregion

        #region COMPRESSION_VERSION_1_SUBFUNCTIONS

        private void VERSION1_SerializeNode(FileDBNode n)
        {
            if (n is Tag)
                VERSION1_SerializeTag((Tag)n);
            else if (n is Attrib)
                VERSION1_SerializeAttrib((Attrib)n);
            writer.Flush();
        }

        private void VERSION1_SerializeTag(Tag t)
        {
            VERSION1_SerializeCollection(t.Children);
        }

        private void VERSION1_SerializeCollection(IEnumerable<FileDBNode> coll)
        {
            foreach (FileDBNode n in coll)
            {
                VERSION1_SerializeNode(n);
            }
            writer.Write((Int16)0);
        }

        private void VERSION1_SerializeAttrib(Attrib a)
        {
            writer.Write((ushort)a.ID);
            writer.Write7BitEncodedInt(a.Bytesize);
            writer.Write(a.Content);
        }

        #endregion

    }
}
