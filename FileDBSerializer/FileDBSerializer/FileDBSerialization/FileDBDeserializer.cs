using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    enum States { Undefined, Tag, Attrib, Terminator }
    public class FileDBDeserializer<T> where T : FileDBDocument, new()
    {
        private BinaryReader reader;
        private T filedb;

        public T Deserialize(String Filename)
        {
            using (var fs = File.OpenRead(Filename))
            using (var ms = new MemoryStream())
            {
                fs.Position = 0;
                fs.CopyTo(ms);
                ms.Position = 0;
                return Deserialize(ms);
            }
        }

        //Main Deserialize Function
        public T Deserialize(Stream s)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            reader = new BinaryReader(s);
            filedb = new T();

            int CurrentLevel = 0;
            Tag CurrentTag = null;

            //at the end of the DOM section, there is an additional closing tag. This means we will end up with CurrentLevel = -1; 
            while (CurrentLevel >= 0)
            {
                int bytesize, ID; 
                States State = ReadOperation(out bytesize, out ID);

                switch (State)
                {
                    //we found attrib
                    case States.Attrib:
                        var attrib = ReadAttrib(bytesize, ID);
                        if (CurrentLevel > 0)
                            CurrentTag.Children.Add(attrib);
                        else
                            filedb.Roots.Add(attrib);
                        break;
                    //we found tag
                    case States.Tag:
                        //Create a new tag and set this one to be the current tag!
                        var Tag = ReadTag(ID, CurrentTag);
                        //make the new tag the current tag and increment

                        if (CurrentLevel > 0)
                            CurrentTag.Children.Add(Tag);
                        else
                            filedb.Roots.Add(Tag);

                        CurrentTag = Tag;
                        CurrentLevel++;
                        break;
                    //we found terminator
                    case States.Terminator:
                        if (CurrentLevel-- > 0)
                            CurrentTag = CurrentTag.Parent;
                        else
                            CurrentTag = null;
                        break;
                    default:
                        throw new InvalidFileDBException(); 
                }
            }

            //init tag section
            TagSection t = ReadTagSection();
            filedb.Tags = t;

            stopWatch.Stop();
            Console.WriteLine("FILEDB Deserialization took {0} ms", stopWatch.Elapsed.TotalMilliseconds);

            return filedb;
        }

        #region VERSION_SPECIFIC

        private States ReadOperation(out int bytesize, out int id)
        {
            if (filedb.VERSION == 2)
            {
                return ReadOperation_VERSION2(out bytesize, out id);
            }
            else if (filedb.VERSION == 1)
            {
                return ReadOperation_VERSION1(out bytesize, out id); 
            }
            //we should not get here
            throw new InvalidFileDBException(); 
        }

        private States ReadOperation_VERSION2(out int _bytesize, out int _id)
        {
            int bytesize = reader.ReadInt32();
            int ID = reader.ReadInt32();
            _bytesize = bytesize;
            _id = ID; 

            switch (bytesize)
            {
                //we found attrib
                case > 0:
                    return States.Attrib;
                //we found tag
                case <= 0 when ID != 0:
                    return States.Tag;
                //we found terminator
                case <= 0 when ID == 0:
                    return States.Terminator;
            }
            return States.Undefined; 
        }

        private States ReadOperation_VERSION1(out int _bytesize, out int _id)
        {
            int ID = reader.ReadUInt16();
            _id = ID;
            _bytesize = 0; 

            switch (ID)
            {
                case > 32768:
                    _bytesize = reader.Read7BitEncodedInt(); 
                    return States.Attrib;
                case <= 32768 when ID != 0:
                    return States.Tag;
                case 0:
                    return States.Terminator;
            }
            return States.Undefined;
        }

        private TagSection ReadTagSection()
        {
            reader.SetPosition(reader.BaseStream.Length - filedb.OFFSET_TO_OFFSETS);
            if (filedb.VERSION == 2)
            {
                int TagsOffset = reader.ReadInt32();
                int AttribsOffset = reader.ReadInt32();
                return new TagSection(
                    /*Tags*/    GetDictionary_VERSION2(TagsOffset),
                    /*Attribs*/ GetDictionary_VERSION2(AttribsOffset)
                );
            }
            else if (filedb.VERSION == 1)
            {
                int TagsOffset = reader.ReadInt32();
                var Tags = GetDictionary_VERSION1(TagsOffset);
                int AttribOffset = (int)reader.Position();
                var Attribs = GetDictionary_VERSION1(AttribOffset);
                return new TagSection() { Tags = Tags, Attribs = Attribs };
            }
            throw new InvalidFileDBException();
        }

        private Dictionary<ushort, String> GetDictionary_VERSION1(int Offset)
        {
            throw new NotImplementedException();
        }

        private Dictionary<ushort, String> GetDictionary_VERSION2(int Offset)
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

        #endregion 


        private Attrib ReadAttrib(int bytesize, int ID)
        {
            int ContentSize = FileDBDocument_V2.GetBlockSpace(bytesize);
            var Content = reader.ReadBytes(ContentSize);
            Array.Resize<byte>(ref Content, bytesize);

            return new Attrib() { ID = ID, Bytesize = bytesize, Content = Content, ParentDoc = filedb };
        }

        private Tag ReadTag(int ID, Tag Parent)
        {
            return new Tag() { ID = ID, ParentDoc = filedb, Parent = Parent };
        }
    }

}
