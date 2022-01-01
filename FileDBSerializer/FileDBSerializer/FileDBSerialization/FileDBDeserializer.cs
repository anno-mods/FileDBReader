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

    /*
     * ///------------------################----------------------///
     * This deserializer holds an Instance of BinaryReader and FileDBDocument.
     * with each call of Deserialize<typeparamref name="T"/>(Stream s), those variables are newly initialized with fresh BinaryReader and FileDBDocument
     * 
     * Modifying those variables outside of the intended deserializing functions will break the entire deserializer. 
     * 
     * ///------------------################----------------------///
     */

    public class FileDBDeserializer<T> where T : IFileDBDocument, new()
    {
        private BinaryReader reader;
        private T filedb;

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
                int bytesize;
                ushort ID;
                States State = ReadOperation(out bytesize, out ID);

                //make sure we don't mess up with the stream. if anything is wrong in the document, 99% of the time we will get a problematic bytesize sooner or later.
                if (bytesize > s.Length - s.Position) 
                {
                    throw new InvalidFileDBException( "bytesize was larger than bytes left to read.");
                }

                switch (State)
                {
                    case States.Attrib:
                        var attrib = ReadAttrib(bytesize, ID);
                        if (CurrentLevel > 0)
                            CurrentTag.Children.Add(attrib);
                        else
                            filedb.Roots.Add(attrib);
                        break;
                    case States.Tag:
                        //Create a new tag;
                        var Tag = ReadTag(ID, CurrentTag);
                        if (CurrentLevel > 0)
                            CurrentTag.Children.Add(Tag);
                        else
                            filedb.Roots.Add(Tag);

                        CurrentTag = Tag;
                        CurrentLevel++;
                        break;
                    case States.Terminator:
                        if (CurrentLevel-- > 0)
                            CurrentTag = CurrentTag.Parent;
                        else CurrentTag = null;
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

        #region ParentFunctions

        /// <summary>
        /// Reads the Next Operation from the Reader and advances its position.
        /// </summary>
        /// <param name="bytesize">out bytesize of the next element: in case of tags and terminators, this is 0</param>
        /// <param name="id">out id of the next element.</param>
        /// <returns>An element of the States enum that describes what the next element is logically. 
        /// 
        /// Tag -> Node that contains other nodes.
        /// Attrib -> Node that contains content data.
        /// Terminator -> closes the current Tag.
        /// 
        /// </returns>

        private States ReadOperation(out int bytesize, out ushort id)
        {
            switch (filedb.VERSION)
            {
                case 1:
                    return ReadOperation_VERSION1(out bytesize, out id);
                case 2:
                    return ReadOperation_VERSION2(out bytesize, out id);
                default:
                    throw new InvalidFileDBException(message: "Unknown FileDB Compression Version");
            }
        }

        private Attrib ReadAttrib(int bytesize, int ID)
        {
            byte[] Content;
            switch (filedb.VERSION)
            {
                case 1:
                    Content = ReadAttribContent_VERSION1(bytesize, ID);
                    break;
                case 2:
                    Content = ReadAttribContent_VERSION2(bytesize, ID);
                    break;
                default:
                    throw new InvalidFileDBException(message: "Unknown FileDB Document Version");
            }
            return new Attrib() { ID = ID, Bytesize = bytesize, Content = Content, ParentDoc = filedb };
        }

        private Tag ReadTag(int ID, Tag Parent)
        {
            return new Tag() { ID = ID, ParentDoc = filedb, Parent = Parent };
        }

        private TagSection ReadTagSection()
        {
            reader.SetPosition(reader.BaseStream.Length - filedb.OFFSET_TO_OFFSETS);
            switch (filedb.VERSION)
            {
                case 1:
                    int TagsOffset = reader.ReadInt32();
                    var Tags = GetDictionary_VERSION1(TagsOffset);
                    int AttribOffset = (int)reader.Position();
                    var Attribs = GetDictionary_VERSION1(AttribOffset);
                    return new TagSection(Tags, Attribs);
                case 2:
                    int TagsOff2 = reader.ReadInt32();
                    int AttribsOffset = reader.ReadInt32();
                    return new TagSection(
                        /*Tags*/    GetDictionary_VERSION2(TagsOff2),
                        /*Attribs*/ GetDictionary_VERSION2(AttribsOffset)
                    );
                default:
                    throw new InvalidFileDBException();
            }
        }
        #endregion

        #region VERSION_SPECIFIC

        private byte[] ReadAttribContent_VERSION1(int bytesize, int ID)
        {
            return reader.ReadBytes(bytesize);
        }
        private byte[] ReadAttribContent_VERSION2(int bytesize, int ID)
        {
            int ContentSize = FileDBDocument_V2.GetBlockSpace(bytesize);
            byte[] Content = reader.ReadBytes(ContentSize);
            Array.Resize<byte>(ref Content, bytesize);
            return Content;
        }


        private States ReadOperation_VERSION2(out int _bytesize, out ushort _id)
        {
            //typecast is not nice. bb fix your shitty compression. 
            int bytesize = reader.ReadInt32();
            ushort ID = (ushort)reader.ReadInt32();
            _bytesize = bytesize;
            _id = ID; 
            return DetermineState(ID); 
        }

        private States ReadOperation_VERSION1(out int _bytesize, out ushort _id)
        {
            ushort ID = reader.ReadUInt16();
            _id = ID;
            _bytesize = 0;
            States State = DetermineState(ID);
            if (State == States.Attrib)
            { 
                _bytesize=reader.Read7BitEncodedInt();
            }
            return State;
        }

        private States DetermineState(int ID)
        {
            switch (ID)
            {
                //we found attrib
                case >= 32768:
                    return States.Attrib;
                //we found tag
                case < 32768 when ID != 0:
                    return States.Tag;
                //we found terminator
                case <= 0:
                    return States.Terminator;
            }
            return States.Undefined;
        }

        private Dictionary<ushort, String> GetDictionary_VERSION1(int Offset)
        {
            reader.BaseStream.Position = Offset;
            Dictionary<ushort, String> dictionary = new Dictionary<ushort, String>();

            int Count = reader.Read7BitEncodedInt();
            for (int i = 0; i < Count; i++)
            {
                String Name = reader.ReadString0();
                var id = reader.ReadUInt16();
                dictionary.Add(id, Name);
            }
            return dictionary;
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
    }

}
