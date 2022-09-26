using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializer.ObjectSerializer.DeserializationHandlers
{
    public class ReferenceArrayHandler : IDeserializationHandler
    {
        public object? Handle(IEnumerable<FileDBNode> nodes, Type targetType, FileDBSerializerOptions options)
        {
            if (nodes.Count() != 1)
                throw new InvalidOperationException("ReferenceArrayHandler can handle exactly one node");
            var node = nodes.First();
            if (node is not Tag tag)
                throw new InvalidOperationException("Only Tags can be handled by ReferenceArrayHandler");

            var actualTargetType = targetType.GetNullableType();
            var arrayContentType = actualTargetType.GetElementType()!;
            var elemCount = tag.Children.Count();
            var itemHandler = HandlerProvider.GetHandlerFor(arrayContentType);

            var arrayInstance = Array.CreateInstance(arrayContentType, elemCount);
            for (int i = 0; i < elemCount; i++)
            {
                var none = tag.Children.ElementAt(i);
                object? arrayEntry = itemHandler.Handle(none.AsEnumerable(), arrayContentType, options);
                arrayInstance.SetValue(arrayEntry, i);
            }
            return arrayInstance;
        }
    }
}
