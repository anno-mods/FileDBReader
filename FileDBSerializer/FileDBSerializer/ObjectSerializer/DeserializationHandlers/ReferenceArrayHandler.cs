using FileDBSerializing;
using FileDBSerializing.LookUps;
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

            if(tag.Children.Count == 0)
            {
                var emptyInstance = Array.CreateInstance(arrayContentType, 0);
                return emptyInstance;
            }

            var elemCountEntry = tag.SelectSingleNode(options.ArraySizeTag);
            if(elemCountEntry is null)
                throw new InvalidOperationException($"The array is missing a size entry when parsing to type {targetType}.");

            if (elemCountEntry is not Attrib sizeAttrib)
                throw new InvalidOperationException("The array size entry must be an Attrib.");

            if(tag.Children.First() != elemCountEntry)
                throw new InvalidOperationException("The array size entry must be the first entry in the Array Tag.");

            int elemCount = (int)PrimitiveTypeConverter.GetObject(typeof(int), sizeAttrib.Content);

            if((elemCount + 1) != tag.Children.Count)
                throw new InvalidOperationException("The array size entry does not match the actual array size.");

            var itemHandler = HandlerProvider.GetHandlerFor(arrayContentType);

            var arrayInstance = Array.CreateInstance(arrayContentType, elemCount);

            if(itemHandler is not TupleHandler)
            {
                for (int i = 0; i < elemCount; i++)
                {
                    var none = tag.Children.ElementAt(i + 1); //Offset by one due to size element
                    object? arrayEntry = itemHandler.Handle(none.AsEnumerable(), arrayContentType, options);
                    arrayInstance.SetValue(arrayEntry, i);
                }
            }
            else
            {
                for (int i = 0; i < elemCount; i++)
                {
                    var nestedWrapper = tag.Children.ElementAt(i + 1);

                    if (nestedWrapper is not Tag nestedTag)
                        throw new InvalidOperationException("A Tuple in a reference array must be a Tag.");

                    int nestedTupleSize = arrayContentType.GetNullableType().GetGenericArguments().Length;

                    if (nestedTag.Children.Count != nestedTupleSize)
                        throw new InvalidOperationException("The nested Tuple Wrapper does not have the correct number of items for the nested Tuple.");

                    object? arrayEntry = itemHandler.Handle(nestedTag.Children, arrayContentType, options);
                    arrayInstance.SetValue(arrayEntry, i);
                }
            }
            return arrayInstance;
        }
    }
}
