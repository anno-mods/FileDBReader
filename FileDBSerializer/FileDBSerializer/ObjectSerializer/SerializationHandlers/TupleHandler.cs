using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AnnoMods.BBDom.ObjectSerializer.SerializationHandlers
{
    internal class TupleHandler : ISerializationHandler
    {
        public IEnumerable<BBNode> Handle(object? item, string tagName, BBDocument workingDocument, BBSerializerOptions options)
        {
            var tuple = item as ITuple;
            if (tuple is null)
                throw new ArgumentException("Tuples to write top FileDB must not be null.");

            for (int i = 0; i < tuple.Length; i++)
            {
                var tuple_entry = tuple[i]!;
                var handler = HandlerProvider.GetHandlerFor(tuple_entry.GetType());

                //return primitives directly, but everthing else should come wrapped.
                if (handler is not TupleHandler)
                {
                    var result = handler.Handle(tuple_entry, tagName, workingDocument, options);
                    foreach (var _ in result)
                        yield return _;
                }
                else
                {
                    var result = handler.Handle(tuple_entry, options.NoneTag, workingDocument, options);
                    var tag = workingDocument.CreateTag(tagName);
                    tag.AddChildren(result);
                    yield return tag;
                }

                
            }
        }
    }
}
