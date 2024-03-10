using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing.ObjectSerializer
{
    public class FileDBSerializerOptions
    {
        public BBDocumentVersion Version { get; set; }
        public String NoneTag { get; } = "None";
        public String ArraySizeTag { get; } = "size";
        public Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

        public bool IgnoreMissingProperties { get; set; } = false;

        public bool SkipDefaultedValues { get; set; } = false;
        public bool SkipSimpleNullValues { get; set; } = true;
        public bool SkipListNullValues { get; set; } = true;
        public bool SkipReferenceArrayNullValues { get; set; } = true;

        public FileDBSerializerOptions()
        {
            Version = BBDocumentVersion.V1;
        }
    }
}
