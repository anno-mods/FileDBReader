using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public class ListHandler : ISerializationHandler
    {
        public IEnumerable<FileDBNode> Handle(object? item, string tagName, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            if(item is null && options.SkipListNullValues)
                return Enumerable.Empty<FileDBNode>();

            Tag t = workingDocument.AddTag(tagName);

            var listInstance = item as IList;
            if (listInstance is null)
                return t.AsEnumerable();

            //Add array entries to the mix
            Type listContentType = listInstance.GetType().GetNullableType().GetGenericArguments().Single();

            foreach (var listEntry in listInstance)
            {
                var itemHandler = HandlerProvider.GetHandlerFor(listContentType);
                var created = itemHandler.Handle(listEntry, options.NoneTag, workingDocument, options);
                foreach (FileDBNode none in created)
                {
                    t.AddChild(none);
                }
            }
            return t.AsEnumerable();
        }
    }
}
