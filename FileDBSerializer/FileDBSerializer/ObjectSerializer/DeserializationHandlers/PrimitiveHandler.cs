using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileDBSerializer.ObjectSerializer.DeserializationHandlers
{
    public class PrimitiveHandler : IDeserializationHandler
    {

        public PrimitiveHandler()
        {
        }

        public object? Handle(IEnumerable<BBNode> nodes, Type targetType, FileDBSerializerOptions options)
        {
            var actualTargetType = targetType.GetNullableType();
            if (nodes.Count() != 1)
                throw new InvalidOperationException("PrimitiveHandler can handle exactly one node");
            var node = nodes.First();
            if (node is not Attrib attrib)
                throw new InvalidOperationException("Only attribs can be handled by PrimitiveHandler");

            return PrimitiveTypeConverter.GetObject(actualTargetType, attrib.Content);
        }
    }
}
