using AnnoMods.BBDom;
using AnnoMods.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace FileDBSerializer.ObjectSerializer.DeserializationHandlers
{
    public class PrimitiveArrayHandler : IDeserializationHandler
    {
        public PrimitiveArrayHandler()
        {
        }

        public object? Handle(IEnumerable<BBNode> nodes, Type targetType, BBSerializerOptions options)
        {
            var actualTargetType = targetType.GetNullableType();
            var elem_type = actualTargetType.GetElementType()!;
            if (nodes.Count() != 1)
                throw new InvalidOperationException("PrimitiveArrayHandler can handle exactly one node");
            var node = nodes.First();
            if (node is not Attrib attrib)
                throw new InvalidOperationException("Only attribs can be handled by PrimitiveArrayHandler");

            int arrayval_size = Marshal.SizeOf(elem_type);
            int array_size = attrib.Bytesize / arrayval_size;

            if (attrib.Bytesize % arrayval_size != 0)
                throw new InvalidOperationException($"Bytesize mismatch: Content size: {attrib.Bytesize}, Individual Element size: {arrayval_size}");

            var arrayInstance = Array.CreateInstance(elem_type, array_size);

            for (int i = 0; i < array_size; i++)
            {
                //todo maybe make this nicer and faster with memory<T>
                var slice = attrib.Content.Skip(i * arrayval_size).Take(arrayval_size);
                var entry = PrimitiveTypeConverter.GetObject(elem_type, slice.ToArray());
                arrayInstance.SetValue(entry, i);
            }

            return arrayInstance;
        }
    }
}
