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
    internal class FlatArrayHandler : ISerializationHandler
    {
        public IEnumerable<FileDBNode> Handle(object? item, string tagName, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            var arrayInstance = item as Array;
            if (arrayInstance is null) 
                yield break;

            Type arrayContentType = arrayInstance.GetType().GetNullableType().GetElementType()!;

            for (int i = 0; i < arrayInstance.Length; i++)
            {
                var arrayEntry = arrayInstance.GetValue(i);
                var itemHandler = HandlerProvider.GetHandlerFor(arrayContentType);

                var created = itemHandler.Handle(arrayEntry, tagName, workingDocument, options);

                foreach(FileDBNode tag in created)
                {
                    yield return tag;
                }
            }
        }
    }
}
