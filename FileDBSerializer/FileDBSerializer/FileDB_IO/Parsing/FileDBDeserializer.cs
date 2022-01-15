using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    /// <summary>
    /// Tag -> Node that contains other nodes
    /// Attrib -> Node that contains content data 
    /// Terminator -> closes the current tag.
    /// </summary>
    public enum States { Undefined, Tag, Attrib, Terminator }

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
        private IFileDBParser parser;

        int CurrentLevel = 0;
        Tag CurrentTag = null;

        //Main Deserialize Function
        public T Deserialize(Stream s)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            reader = new BinaryReader(s);
            filedb = new T();

            switch (filedb.VERSION)
            {
                case 1: parser = new FileDBParser_V1(reader, filedb); break;
                case 2: parser = new FileDBParser_V2(reader, filedb); break;
            }

            CurrentLevel = 0;
            CurrentTag = null;

            //at the end of the DOM section, there is an additional closing tag. This means we will end up with CurrentLevel = -1; 
            while (CurrentLevel >= 0)
            {
                States State = parser.ReadNextOperation(out int bytesize, out ushort ID);

                //make sure we don't mess up with the stream. if anything is wrong in the document, 99% of the time we will get a problematic bytesize sooner or later.
                if (bytesize > s.Length - s.Position) 
                {
                    throw new InvalidFileDBException("bytesize was larger than bytes left to read.");
                }

                switch (State)
                {
                    case States.Attrib:
                        var attrib = parser.ReadAttrib(bytesize, ID);
                        AddNode(attrib);
                        break;
                    case States.Tag:
                        //Create a new tag;
                        var Tag = parser.ReadTag(ID, CurrentTag);
                        AddNode(Tag);
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
            TagSection t = parser.ReadTagSection(filedb.OFFSET_TO_OFFSETS);
            filedb.Tags = t;

            stopWatch.Stop();
            Console.WriteLine("FILEDB Deserialization took {0} ms", stopWatch.Elapsed.TotalMilliseconds);
            return filedb;
        }

        private void AddNode(FileDBNode node)
        {
            if (CurrentLevel > 0)
                CurrentTag.Children.Add(node);
            else
                filedb.Roots.Add(node);
        }

        
    }

}
