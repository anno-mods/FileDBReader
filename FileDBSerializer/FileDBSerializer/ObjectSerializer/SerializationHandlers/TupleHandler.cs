using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    internal class TupleHandler : ISerializationHandler
    {
        public IEnumerable<FileDBNode> Handle(object graph, PropertyInfo property, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
