using AnnoMods.BBDom;
using AnnoMods.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public class ReferenceArrayHandler : ISerializationHandler
    {
        public IEnumerable<BBNode> Handle(object? item, string tagName, IBBDocument workingDocument, BBSerializerOptions options)
        {
            if (item is null && options.SkipReferenceArrayNullValues)
                return Enumerable.Empty<BBNode>();

            Tag t = workingDocument.CreateTag(tagName);

            var arrayInstance = item as Array;
            if (arrayInstance is null) 
                return t.AsEnumerable();

            var size = arrayInstance.Length;

            //Empty Reference Array does not need a <size></size> tag/attrib
            if (size == 0)
                return t.AsEnumerable();

            var size_node = workingDocument.CreateAttrib(options.ArraySizeTag);
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
                    var none = workingDocument.CreateTag(options.NoneTag);
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
