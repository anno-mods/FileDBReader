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
                foreach (FileDBNode n in filedb.Roots)
                {
                    this.SerializeNode(n);
                }
                writer.Write((Int64)0);

                stopWatch.Stop();
                Console.WriteLine("FILEDB Serialization took: " + stopWatch.Elapsed.TotalMilliseconds);

                //todo serialize tag section
                return ms;
            }
        }

        private void SerializeNode(FileDBNode n)
        {
            if (n is Tag)
                SerializeTag((Tag)n);
            else if (n is Attrib)
                SerializeAttrib((Attrib)n);
            writer.Flush();
        }

        private void SerializeTag(Tag t)
        {
            writer.Write(t.Bytesize);
            writer.Write(t.ID);
            foreach (FileDBNode n in t.Children)
            {
                SerializeNode(n);
            }
            writer.Write((Int64)0);
        }

        private void SerializeAttrib(Attrib a)
        {
            writer.Write(a.Bytesize);
            writer.Write(a.ID);
            writer.Write(GetAttribInBlocks(a.Content, a.Bytesize));
        }

        private byte[] GetAttribInBlocks(byte[] attrib, int bytesize)
        {
            int ContentSize = FileDBDocument.GetBlockSpace(bytesize);
            Array.Resize<byte>(ref attrib, ContentSize);
            return attrib; 
        }
        #endregion

    }
}
