using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public class ReferenceArrayHandler : ISerializationHandler
    {
        public IEnumerable<FileDBNode> Handle(object? item, string tagName, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            if (item is null && options.SkipReferenceArrayNullValues)
                return Enumerable.Empty<FileDBNode>();

            Tag t = workingDocument.AddTag(tagName);

            var arrayInstance = item as Array;
            if (arrayInstance is null) 
                return t.AsEnumerable();

            var size = arrayInstance.Length;
            var size_node = workingDocument.AddAttrib(options.ArraySizeTag);
            size_node.Content = BitConverter.GetBytes(size);
            t.AddChild(size_node);

            //Add array entries to the mix
            Type arrayContentType = arrayInstance.GetType().GetNullableType().GetElementType()!;
            var itemHandler = HandlerProvider.GetHandlerFor(arrayContentType);

            for (int i = 0; i < arrayInstance.Length; i++)
            {
                var arrayEntry = arrayInstance.GetValue(i);
                var created = itemHandler.Handle(arrayEntry, options.NoneTag, workingDocument, options);

                if (itemHandler is TupleHandler)
                {
                    var none = workingDocument.AddTag(options.NoneTag);
                    none.AddChildren(created);
                    t.AddChild(none);
                }
                else
                    t.AddChildren(created);
            }
            return t.AsEnumerable();
        }
    }
}
