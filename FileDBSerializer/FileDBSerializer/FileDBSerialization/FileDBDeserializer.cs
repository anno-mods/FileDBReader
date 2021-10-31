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

        public FileDBDocument Deserialize(String Filename)
        {
            using (var fs = File.OpenRead(Filename))
            {
                return Deserialize(fs);
            }
        }

        public FileDBDocument Deserialize(Stream s)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            reader = new BinaryReader(s);
            FileDBDocument filedb = new FileDBDocument(); 

            int CurrentLevel = 0;
            Tag Root = new Tag(); 
            Tag CurrentTag = Root;

            //at the end of the DOM section, there is an additional closing tag 0x0000000000000000. This means we will end up with CurrentLevel = -1; 
            while (CurrentLevel >= 0)
            {
                int bytesize = reader.ReadInt32();
                int ID = reader.ReadInt32();

                switch (bytesize)
                {
                    //we found attrib
                    case > 0 :
                            int ContentSize = FileDBDocument.GetBlockSpace(bytesize);
                            var Content = reader.ReadBytes(ContentSize);
                            Array.Resize<byte>(ref Content, bytesize);

                            Attrib attrib = new Attrib() { ID = ID, Bytesize = bytesize, Content = Content, ParentDoc = filedb };
                            CurrentTag.Children.Add(attrib);
                            break;
                    //we found tag
                    case <= 0 when ID != 0:
                            //Create a new tag and set this one to be the current tag!
                            Tag Tag = new Tag() { Bytesize = bytesize, ID = ID, ParentDoc = filedb, Parent = CurrentTag };

                            //make the new tag the current tag and increment
                            CurrentTag.Children.Add(Tag);
                            CurrentTag = Tag;
                            CurrentLevel++;
                        break;
                    //we found terminator
                    case <= 0 when ID == 0:
                            if (CurrentLevel-- >= 0)
                                CurrentTag = CurrentTag.Parent;
                        break; 
                }
            }

            //Read Tag Section 
            reader.BaseStream.Position = reader.BaseStream.Length - FileDBDocument.OFFSET_TO_OFFSETS;
            int TagsOffset = reader.ReadInt32();
            int AttribsOffset = reader.ReadInt32();

            //init tag section
            TagSection t = ReadTagSection(TagsOffset, AttribsOffset);
            filedb.Tags = t; 

            stopWatch.Stop();
            Console.WriteLine("FILEDB Deserialization took: " + stopWatch.Elapsed.TotalMilliseconds);

            //we are copying the entire roots here just to save a few lines of code :D
            filedb.Roots = Root.Children; 
            return filedb;
        }

        private TagSection ReadTagSection( int TagsOffset, int AttribsOffset)
        {
            return new TagSection(
                /*Tags*/    GetDictionary(TagsOffset), 
                /*Attribs*/ GetDictionary(AttribsOffset)
            );
        }

        private Dictionary<ushort, String> GetDictionary(int Offset)
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
    }
}
