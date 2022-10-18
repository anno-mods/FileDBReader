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
    public class FlatArrayHandler : IDeserializationHandler
    {
        public object? Handle(IEnumerable<FileDBNode> nodes, Type targetType, FileDBSerializerOptions options)
        {
            var actualTargetType = targetType.GetNullableType();
            var listContentType = actualTargetType.GetGenericArguments().Single()!;

            var elemCount = nodes.Count();
            var itemHandler = HandlerProvider.GetHandlerFor(listContentType);

            var listConstructor = actualTargetType.GetConstructor(new Type[] { });
            var listInstance = (IList)listConstructor!.Invoke(new object[] { });

            if (itemHandler is not TupleHandler)
            {
                for (int i = 0; i < elemCount; i++)
                {
                    var none = nodes.ElementAt(i);
                    object? listEntry = itemHandler.Handle(none.AsEnumerable(), listContentType, options);
                    listInstance.Add(listEntry);
                }
            }
            else
            {
                int stride = listContentType.GetNullableType().GetGenericArguments().Length;
                if (elemCount % stride != 0)
                    throw new InvalidOperationException("The amount of items in this List of Tuples cannot properly fill Tuples without remainder.\r\n" +
                        $"There are {elemCount % stride} items too many.");

                for (int i = 0; i < elemCount; i += stride)
                {
                    FileDBNode[] tupleItems = new FileDBNode[stride];
                    for (int j = 0; j < tupleItems.Length; j++)
                    {
                        tupleItems[j] = nodes.ElementAt(i + j);
                    }

                    object? listEntry = itemHandler.Handle(tupleItems, listContentType, options);
                    listInstance.Add(listEntry);
                }

            }
            return listInstance;
        }
    }
}
