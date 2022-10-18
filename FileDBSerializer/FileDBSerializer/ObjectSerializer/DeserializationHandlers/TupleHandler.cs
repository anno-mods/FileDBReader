using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializer.ObjectSerializer.DeserializationHandlers
{
    public class TupleHandler : IDeserializationHandler
    {
        public object? Handle(IEnumerable<FileDBNode> nodes, Type targetType, FileDBSerializerOptions options)
        {
            var actualTargetType = targetType.GetNullableType();
            var tupleContentTypes = actualTargetType.GetGenericArguments();
            int tupleSize = tupleContentTypes.Length;

            if (nodes.Count() != tupleSize)
                throw new InvalidOperationException("TupleHandler must receive the exact amount of nodes that the tuple type has as generic arguments.\r\n" +
                    $"Expected {tupleSize} but got {nodes.Count()}.");

            object?[] items = new object[tupleSize];

            for (int i = 0;i < tupleSize; i++)
            {
                var itemHandler = HandlerProvider.GetHandlerFor(tupleContentTypes[i]);
                if(itemHandler is not TupleHandler)
                {
                    var none = nodes.ElementAt(i);
                    object? tupleItem = itemHandler.Handle(none.AsEnumerable(), tupleContentTypes[i], options);
                    items[i] = tupleItem;
                }
                else
                {
                    var nestedWrapper = nodes.ElementAt(i);

                    if(nestedWrapper is not Tag nestedTag)
                        throw new InvalidOperationException("A nested Tuple Wrapper must be a Tag.");

                    int nestedTupleSize = tupleContentTypes[i].GetNullableType().GetGenericArguments().Length;

                    if(nestedTag.Children.Count != nestedTupleSize)
                        throw new InvalidOperationException("The nested Tuple Wrapper does not have the correct number of items for the nested Tuple.");

                    object? tupleItem = itemHandler.Handle(nestedTag.Children, tupleContentTypes[i], options);
                    items[i] = tupleItem;
                }
            }

            var tupleConstructor = actualTargetType.GetConstructor(tupleContentTypes);
            var tupleInstance = tupleConstructor!.Invoke(items);

            return tupleInstance;
        }
    }
}
