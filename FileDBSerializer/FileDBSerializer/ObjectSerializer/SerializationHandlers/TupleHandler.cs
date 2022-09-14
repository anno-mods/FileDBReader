using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    internal class TupleHandler : ISerializationHandler
    {
        public IEnumerable<FileDBNode> Handle(object? item, string tagName, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            var tuple = item as ITuple;
            if (tuple is null)
                throw new Exception();

            for (int i = 0; i < tuple.Length; i++)
            {
                var tuple_entry = tuple[i];
                var handler = HandlerProvider.GetHandlerFor(tuple_entry.GetType(), Enumerable.Empty<Attribute>());
                var result = handler.Handle(tuple_entry, tagName, workingDocument, options);

                foreach (var _ in result)
                {
                    yield return _;
                }

            }
        }
    }
}
