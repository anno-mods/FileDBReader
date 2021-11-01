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
         * This deserializer holds an Instance of BinaryWriter
         * with each call of Serialize, those variable is newly initialized with a fresh BinaryReader
         * 
         * Modifying this variable outside of the intended serializing functions will break the entire serializer. 
         * 
         * ///------------------################----------------------///
         */

        private BinaryWriter writer;

        #region serialize
        /// <summary>
        /// Serializes a FileDBDocument to a Stream.
        /// </summary>
        /// <param name="filedb"></param>
        /// <param name="s">The Stream that should be serialized to.</param>
        /// <returns>the UNCLOSED Stream that contains a serialized version of the document</returns>
        public Stream Serialize (FileDBDocument filedb, Stream s)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            writer = new BinaryWriter(s);
            if (filedb.VERSION == 2)
            {
                VERSION2_SerializeCollection(filedb.Roots);
                VERSION2_SerializeTagSection(filedb.Tags);
                writer.Write(FileDBDocument_V2._magic_bytes);
                writer.Flush();
            }
            else if (filedb.VERSION == 1)
            {
                VERSION1_SerializeCollection(filedb.Roots);
                VERSION1_SerializeTagSection(filedb.Tags);
                writer.Flush();
            }
            s.Position = 0;
            stopWatch.Stop();
            Console.WriteLine("FILEDB Serialization took {0} ms", stopWatch.Elapsed.TotalMilliseconds);
            return s;
        }

        #endregion

        #region COMPRESSION_VERSION_2_SUBFUNCTIONS

        
        private void VERSION2_SerializeTagSection(TagSection t)
        {
            //remove None Tags
            t.Tags.Remove(1);
            t.Attribs.Remove(32768);

            int TagsOffset = VERSION2_SerializeDictionary(t.Tags);
            int AttribsOffset = VERSION2_SerializeDictionary(t.Attribs);
            writer.Write(TagsOffset);
            writer.Write(AttribsOffset);
            writer.Flush();

            //readd None Tags
            t.Tags.Add(1, "None");
            t.Attribs.Add(32768, "None");
        }

        /// <summary>
        /// Serializes a dictionary 
        /// </summary>
        /// <param name="t"></param>
        /// <returns>The in file offset to the tag section </returns>
        private int VERSION2_SerializeDictionary(Dictionary<ushort, String> dict)
        {
            int Offset = (int)writer.BaseStream.Position;
            writer.Write(dict.Count);
            int bytesWritten = 4;
            foreach (KeyValuePair<ushort, String> k in dict)
            {
                writer.Write(k.Key);
                bytesWritten += 2; 
            }
            foreach (KeyValuePair<ushort, String> k in dict)
            {
                bytesWritten += writer.WriteString0(k.Value);
            }

            //fill each dictionary size up to a multiple of 8.
            int ToWrite = FileDBDocument_V2.GetBlockSpace(bytesWritten) - bytesWritten;
            var AddBytes = new byte[ToWrite];
            writer.Write(AddBytes);
            writer.Flush(); 
            return Offset; 
        }

        private void VERSION2_SerializeNode(FileDBNode n)
        {
            writer.Write(n.Bytesize);
            writer.Write(n.ID);

            if (n.NodeType == FileDBNodeType.Tag)
                VERSION2_SerializeTag((Tag)n);
            else if (n.NodeType == FileDBNodeType.Attrib)
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
            writer.Write(FileDBDocument_V2.GetNodeTerminator());
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
            writer.Write( FileDBDocument_V2.GetNodeTerminator());
        }

        private void VERSION1_SerializeAttrib(Attrib a)
        {
            writer.Write((ushort)a.ID);
            writer.Write7BitEncodedInt(a.Bytesize);
            writer.Write(a.Content);
        }

        private void VERSION1_SerializeTagSection(TagSection t)
        {
            int offset = VERSION1_SerializeDictionary(t.Tags);
            VERSION1_SerializeDictionary(t.Attribs);
            writer.Write(offset);
        }


        private int VERSION1_SerializeDictionary(Dictionary<ushort, String> dict)
        {
            int offset = (int)writer.Position(); 
            writer.Write7BitEncodedInt(dict.Count);
            foreach (KeyValuePair<ushort, String> k in dict)
            {
                writer.WriteString0(k.Value);
                writer.Write(k.Key);
            }
            return offset;
        }

        #endregion

    }
}
