using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Syroot.BinaryData.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;

namespace FileDBReader {

    /// <summary>
    /// FileReader done by @VeraAtVersus
    /// 
    /// Converts a FileDB Compressed file to an xml representation with data represented in hex strings.
    /// </summary>
  public class FileReader {

    #region Properties

    public Dictionary<ushort, string> Tags { get; set; } = new Dictionary<ushort, string>();

        #endregion Properties

        #region Methods
        public XDocument ReadSpan(Span<byte> SpanToRead)
        {
            //Init
            Tags.Clear();
            Tags.Add(1, "None");
            Tags.Add(32768, "None");

            var document = new XDocument();
            var root = new XElement("Content");
            document.Add(root);
            XElement currentNode = null;

            var Filereader = new SpanReader(SpanToRead);

            //Check Compression
            if (Filereader.ReadInt16() == -9608)
            {
                Filereader = new SpanReader(DecompressSpan(Filereader.Span));
            }

            //Set Position from The Tags Section
            var TagsOff = Filereader.Position = Convert.ToInt32(MemoryMarshal.Read<UInt32>(Filereader.Span[^4..]));


            //this fails for the internal filedb???
            //Read Tags
            ExtractTags(ref Filereader, Tags);
            //Read Attributes
            ExtractTags(ref Filereader, Tags);

            //Get Nodes Section
            var nodesReader = Filereader[..TagsOff];
            var count = 0;
            while (nodesReader.Position < nodesReader.Length)
            {
                //Little Output
                count++;
                if ((count & 1000) == 1000)
                {
                    Console.WriteLine($"{nodesReader.Position} - {nodesReader.Length - nodesReader.Position}");
                }

                //Get Next ID
                nodesReader.ReadUInt16(out var nextId);

                //Close Node
                if (nextId == 0)
                {
                    currentNode = currentNode?.Parent == document.Root ? null : currentNode?.Parent;
                }
                //Check for Existing Id
                else if (Tags.TryGetValue(nextId, out var tag))
                {
                    //Tag
                    if (nextId < 32768)
                    {
                        var node = new XElement(tag);
                        AddToCurrentNode(document, currentNode, node);
                        currentNode = node;
                    }

                    //Attribute
                    else
                    {
                        nodesReader.ReadInt32Bit7(out var length);
                        var attribute = new XElement(tag);

                        //interpretation happens laterz
                        object content;
                        {
                            content = nodesReader.Span.Slice(nodesReader.Position, length).ToHexString();
                            nodesReader.Position += length;
                        }


                        attribute = new XElement(tag, content);
                        AddToCurrentNode(document, currentNode, attribute);
                    }
                }
            }
            return document;
        }

        public XmlDocument ReadFile(string path) {
            Span<byte> Span = File.ReadAllBytes(path).AsSpan();
            var document = ReadSpan(Span);

            var xml = new XmlDocument();
            using (var xmlReader = document.CreateReader())
            {
                xml.Load(xmlReader);
                return xml;
            }
        }

    private static void ExtractTags(ref SpanReader reader, Dictionary<ushort, string> dictionary) {
      var count = reader.ReadInt32Bit7();
      for (var i = 0; i < count; i++) {
        reader.ReadString0(out var name);
        reader.ReadUInt16(out var id);

        //Xml Space in Name Fix
        dictionary.Add(id, name.Replace(" ", "_"));
      }
    }

    private static void AddToCurrentNode(XDocument document, XElement currentNode, XElement node) {
      //if nothing 
      if (currentNode == null) {
        //var newnode = new XElement("Base");
        //document.Root.Add(newnode);
        currentNode = document.Root;
      }
      currentNode.Add(node);
    }
    public byte[] DecompressSpan(ReadOnlySpan<byte> bytesToDecompress) {
      using var memStream = new MemoryStream();
      memStream.Write(bytesToDecompress);
      using var decompressionStream = new InflaterInputStream(memStream);
      using var decompressedFileStream = new MemoryStream();
      memStream.Position = 0;
      decompressionStream.CopyTo(decompressedFileStream);

      return decompressedFileStream.ToArray();
    }

    #endregion Methods
  }
}