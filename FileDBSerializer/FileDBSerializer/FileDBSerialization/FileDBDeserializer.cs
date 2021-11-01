using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    public class FileDBDeserializer
    {
        BinaryReader reader;

        #region COMPRESSION_VERSION_2_PARENT_FUNCTIONS

        public FileDBDocument_V2 VERSION2_Deserialize(String Filename)
        {
            using (var fs = File.OpenRead(Filename))
            {
                return VERSION2_Deserialize(fs);
            }
        }

        public FileDBDocument_V2 VERSION2_Deserialize(Stream s)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            reader = new BinaryReader(s);
            FileDBDocument_V2 filedb = new FileDBDocument_V2();

            int CurrentLevel = 0;
            Tag CurrentTag = null;

            //at the end of the DOM section, there is an additional closing tag 0x0000000000000000. This means we will end up with CurrentLevel = -1; 
            while (CurrentLevel >= 0)
            {
                int bytesize = reader.ReadInt32();
                int ID = reader.ReadInt32();

                switch (bytesize)
                {
                    //we found attrib
                    case > 0:
                        var attrib = VERSION2_ReadAttrib(bytesize, ID, filedb);
                        if (CurrentLevel > 0)
                            CurrentTag.Children.Add(attrib);
                        else
                            filedb.Roots.Add(attrib);
                        break;
                    //we found tag
                    case <= 0 when ID != 0:
                        //Create a new tag and set this one to be the current tag!
                        var Tag = VERSION2_ReadTag(ID, filedb, CurrentTag);
                        //make the new tag the current tag and increment

                        if (CurrentLevel > 0)
                            CurrentTag.Children.Add(Tag);
                        else
                            filedb.Roots.Add(Tag);

                        CurrentTag = Tag;
                        CurrentLevel++;
                        break;
                    //we found terminator
                    case <= 0 when ID == 0:
                        if (CurrentLevel-- > 0)
                            CurrentTag = CurrentTag.Parent;
                        else
                            CurrentTag = null; 
                        break;
                }
            }

            //Read Tag Section Offsets.
            reader.BaseStream.Position = reader.BaseStream.Length - FileDBDocument_V2.OFFSET_TO_OFFSETS;
            int TagsOffset = reader.ReadInt32();
            int AttribsOffset = reader.ReadInt32();

            //init tag section
            TagSection t = VERSION2_ReadTagSection(TagsOffset, AttribsOffset);
            filedb.Tags = t;

            stopWatch.Stop();
            Console.WriteLine("FILEDB Deserialization took: " + stopWatch.Elapsed.TotalMilliseconds);

            return filedb;
        }

        #endregion

        #region COMPRESSION_VERSION_2_SUBFUNCTIONS
        private TagSection VERSION2_ReadTagSection(int TagsOffset, int AttribsOffset)
        {
            return new TagSection(
                /*Tags*/    VERSION2_GetDictionary(TagsOffset),
                /*Attribs*/ VERSION2_GetDictionary(AttribsOffset)
            );
        }

        private Dictionary<ushort, String> VERSION2_GetDictionary(int Offset)
        {
            reader.BaseStream.Position = Offset;
            Dictionary<ushort, string> dictionary = new Dictionary<ushort, string>();

            int Count = reader.ReadInt32();
            ushort[] IDs = new ushort[Count];

            for (int i = 0; i < Count; i++)
            {
                IDs[i] = reader.ReadUInt16();
            }
            for (int i = 0; i < Count; i++)
            {
                String name = reader.ReadString0();
                dictionary.Add(IDs[i], name);
            }
            return dictionary;
        }

        private Attrib VERSION2_ReadAttrib(int bytesize, int ID, FileDBDocument ParentDoc)
        {
            int ContentSize = FileDBDocument_V2.GetBlockSpace(bytesize);
            var Content = reader.ReadBytes(ContentSize);
            Array.Resize<byte>(ref Content, bytesize);

            return new Attrib() { ID = ID, Bytesize = bytesize, Content = Content, ParentDoc = ParentDoc };
        }

        private Tag VERSION2_ReadTag(int ID, FileDBDocument parentDoc, Tag Parent)
        {
            return new Tag() { ID = ID, ParentDoc = parentDoc, Parent = Parent };
        }

        #endregion

        #region COMPRESSION_VERSION_1_SUBFUNCTIONS

        private Attrib VERSION1_ReadAttrib(int ID, FileDBDocument ParentDoc)
        {
            var bytesize = reader.Read7BitEncodedInt();
            byte[] Content = reader.ReadBytes(bytesize);

            return new Attrib() { ID = ID, Bytesize = bytesize, Content = Content, ParentDoc = ParentDoc };
        }

        private Tag VERSION1_ReadTag(int ID, FileDBDocument parentDoc, Tag Parent)
        {
            return new Tag() { ID = ID, ParentDoc = parentDoc, Parent = Parent };
        }

        private TagSection VERSION1_ReadTagSection(int TagsOffset, int AttribsOffset)
        {
            return new TagSection(
                /*Tags*/    VERSION1_GetDictionary(TagsOffset),
                /*Attribs*/ VERSION1_GetDictionary(AttribsOffset)
            );
        }

        private Dictionary<ushort, String> VERSION1_GetDictionary(int Offset)
        {
            reader.BaseStream.Position = Offset;
            Dictionary<ushort, string> dictionary = new Dictionary<ushort, string>();
            var Count = reader.Read7BitEncodedInt();
            for (var i = 0; i < Count; i++)
            {
                var name = reader.ReadString0();
                var id = reader.ReadUInt16();
                dictionary.Add(id, name);
            }
            return dictionary;
        }


        #endregion

    }
}
