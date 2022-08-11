using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBReader.src
{
    public class StreamExtensions
    {
        public static void CopyToFully(Stream source, Stream target)
        {
            target.Position = 0;
            source.Position = 0;
            source.CopyTo(target);
            source.Position = 0;
        }
    }
}
