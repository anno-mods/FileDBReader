using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace FileDBSerializing.Tests.TestData
{
    public class RootObject
    {
        public int RootCount { get; set; }
        public SomethingManager DumbManager { get; set; }
        public ChildElement DumbChild { get; set; }

        public int[] Map { get; set; }
    }
}
