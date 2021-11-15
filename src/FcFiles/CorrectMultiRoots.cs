using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace FileDBReader.src
{
    public class CorrectMultiRoots
    {
        String DummyRootName = "Content";
        public Stream AddMultiRoot(Stream s)
        {
            long PositionPreOperation = s.Position;

            MemoryStream ms = new MemoryStream();
            ms.Write(Encoding.UTF8.GetBytes("<"+DummyRootName+">"));
            s.Position = 0;
            s.CopyTo(ms);
            ms.Write(Encoding.UTF8.GetBytes("</" + DummyRootName + ">"));
            s.Position = PositionPreOperation;
            ms.Position = 0;
            return ms;
        }

        public Stream RemoveMultiRoot(Stream inputDoc)
        {
            var doc = new XmlDocument();
            try
            {
                doc.Load(inputDoc);
            } catch  {
                Console.WriteLine("[MULTIROOTS]: Input Document needs to be valid xml.");
            }

            if (doc.FirstChild == null) throw new ArgumentException("[MULTIROOTS]: The XML Document lacks some actual content.");

            Stream s = new MemoryStream();
            StreamWriter writer = new StreamWriter(s);

            foreach (XmlNode n in doc.FirstChild.ChildNodes)
            {
                XmlDocument d = new XmlDocument();
                var importN = d.ImportNode(n, true);
                d.AppendChild(importN);
                d.Save(s);
                writer.Write("\n");
            }
            return s;
        }
    }
}
