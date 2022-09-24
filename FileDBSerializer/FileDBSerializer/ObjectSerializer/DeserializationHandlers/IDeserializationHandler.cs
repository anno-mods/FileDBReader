using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializer.ObjectSerializer.DeserializationHandlers
{
    public interface IDeserializationHandler
    {
        public object? Handle(IEnumerable<FileDBNode> nodes, Type targetType, FileDBSerializerOptions options);
    }
}
