using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    internal class FlatArrayHandler : ISerializationHandler
    {
        public IEnumerable<FileDBNode> Handle(object? item, string tagName, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            var listInstance = item as IList;
            if (listInstance is null) 
                yield break;

            Type listContentType = listInstance.GetType().GetNullableType().GetGenericArguments().Single();

            foreach (var listEntry in listInstance)
            {
                var itemHandler = HandlerProvider.GetHandlerFor(listContentType);

                var created = itemHandler.Handle(listEntry, tagName, workingDocument, options);

                foreach(FileDBNode tag in created)
                {
                    yield return tag;
                }
            }
        }
    }
}
