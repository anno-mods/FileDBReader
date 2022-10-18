using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializer.ObjectSerializer.DeserializationHandlers
{
    public class ListHandler : IDeserializationHandler
    {
        public object? Handle(IEnumerable<FileDBNode> nodes, Type targetType, FileDBSerializerOptions options)
        {
            if (nodes.Count() != 1)
                throw new InvalidOperationException("ListHandler can handle exactly one node");
            var node = nodes.First();
            if (node is not Tag tag)
                throw new InvalidOperationException("Only Tags can be handled by ListHandler");

            //Add array entries to the mix
            var actualTargetType = targetType.GetNullableType();
            var listContentType = actualTargetType.GetGenericArguments().Single()!;

            var elemCount = tag.Children.Count();
            var itemHandler = HandlerProvider.GetHandlerFor(listContentType);

            var listConstructor = actualTargetType.GetConstructor(new Type[] { });
            var listInstance = (IList)listConstructor!.Invoke(new object[] { });

            for (int i = 0; i < elemCount; i++)
            {
                var none = tag.Children.ElementAt(i);
                object? listEntry = itemHandler.Handle(none.AsEnumerable(), listContentType, options);
                listInstance.Add(listEntry);
            }
            return listInstance;
        }
    }
}
