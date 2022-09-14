using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public class TupleListHandler : ISerializationHandler
    {
        public IEnumerable<FileDBNode> Handle(object? item, string tagName, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            Tag t = workingDocument.AddTag(tagName);

            var listInstance = item as IList;
            if (listInstance is null)
                return t.AsEnumerable();

            //Add array entries to the mix
            Type listContentType = listInstance.GetType().GetNullableType().GetGenericArguments().Single();

            foreach (var listEntry in listInstance)
            {
                var none = workingDocument.AddTag(options.NoneTag);
                var itemHandler = HandlerProvider.GetHandlerFor(listContentType, new List<Attribute>());
                var created = itemHandler.Handle(listEntry, options.NoneTag, workingDocument, options);
                foreach (FileDBNode entry in created)
                {
                    none.AddChild(entry);
                }
                t.AddChild(none);
            }
            return t.AsEnumerable();
        }
    }
}
