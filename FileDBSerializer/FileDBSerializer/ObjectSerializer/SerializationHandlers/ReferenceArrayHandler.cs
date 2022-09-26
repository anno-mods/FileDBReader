using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public class ReferenceArrayHandler : ISerializationHandler
    {
        public IEnumerable<FileDBNode> Handle(object? item, string tagName, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            Tag t = workingDocument.AddTag(tagName);

            var arrayInstance = item as Array;
            if (arrayInstance is null) 
                return t.AsEnumerable();

            //Add array entries to the mix
            Type arrayContentType = arrayInstance.GetType().GetNullableType().GetElementType()!;
            var itemHandler = HandlerProvider.GetHandlerFor(arrayContentType);

            for (int i = 0; i < arrayInstance.Length; i++)
            {
                var arrayEntry = arrayInstance.GetValue(i);
                var created = itemHandler.Handle(arrayEntry, options.NoneTag, workingDocument, options);

                foreach (FileDBNode none in created)
                {
                    t.AddChild(none);
                }
            }
            return t.AsEnumerable();
        }
    }
}
