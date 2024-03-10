using System;
using System.Diagnostics;
using System.IO;

namespace AnnoMods.BBDom.IO
{
    /// <summary>
    /// Tag -> Node that contains other nodes
    /// Attrib -> Node that contains content data 
    /// Terminator -> closes the current tag.
    /// </summary>
    public enum States
{
    Undefined,
    Tag,
    Attrib,
    Terminator
}

public class BBDocumentParser
{
    private BBDocument bbdoc;
    private IBBStructureParser parser;

    int CurrentLevel = 0;
    Tag? CurrentTag = null;
    Stream? CurrentStream = null;

    public BBDocumentVersion Version { get; }

    public BBDocumentParser(BBDocumentVersion version)
    {
        Version = version;
    }

    private void Init(Stream source)
    {
        if (!source.CanRead) throw new ArgumentException("Stream needs to be readable!");
        CurrentStream = source;
        bbdoc = new BBDocument();
        parser = DependencyVersions.GetParser(Version, source);
        parser.RegisterDocument(bbdoc);
    }

    public BBDocument LoadBBDocument(Stream s)
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        Init(s);
        try
        {
            ParseDOMSection();
            ParseTagSection();
        }
        catch (IOException ex)
        {
            throw new InvalidBBException("Reached end of file while parsing");
        }

        stopWatch.Stop();
        Console.WriteLine("BB Deserialization took {0} ms", stopWatch.Elapsed.TotalMilliseconds);
        return bbdoc;
    }

    private void ParseTagSection()
    {
        try
        {
            var offset = Versioning.GetOffsetToOffsets(Version);
            TagSection t = parser.ReadTagSection(offset);
            bbdoc.TagSection = t;
        }
        catch (ArgumentException e)
        {
            throw new InvalidBBException("Definition of Tag Dictionary is invalid!");
        }
    }

    private void ParseDOMSection()
    {
        CurrentLevel = 0;
        CurrentTag = null;

        //at the end of the DOM section, there is an additional closing tag. This means we will end up with CurrentLevel = -1; 
        while (CurrentLevel >= 0)
        {
            States State = parser.ReadNextOperation(out int bytesize, out ushort ID);

            //make sure we don't mess up with the stream. if anything is wrong in the document, 99% of the time we will get a problematic bytesize sooner or later.
            if (bytesize > CurrentStream?.Length - CurrentStream?.Position)
            {
                throw new InvalidBBException("bytesize was larger than bytes left to read.");
            }

            switch (State)
            {
                case States.Attrib:
                    var attrib = parser.ReadAttrib(bytesize, ID, CurrentTag);
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
                    throw new InvalidBBException("Undefined State");
            }
        }
    }

    private void AddNode(BBNode node)
    {
        if (CurrentLevel > 0)
            CurrentTag.Children.Add(node);
        else
            bbdoc.Roots.Add(node);
    }


}

}
