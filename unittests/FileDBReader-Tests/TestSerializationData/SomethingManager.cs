using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace FileDBSerializing.Tests.TestData
{
    public class SomethingManager
    {
        public int SomethingCount { get; set; }
        public float SomethingValue { get; set; }
        public ChildElement Child { get; set; }
    }
}
