using AnnoMods.BBDom;
using AnnoMods.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializer.ObjectSerializer.DeserializationHandlers
{
    public class StringHandler : IDeserializationHandler
    {
        public object? Handle(IEnumerable<BBNode> nodes, Type targetType, BBSerializerOptions options)
        {
            var actualTargetType = targetType.GetNullableType();
            if (nodes.Count() != 1)
                throw new InvalidOperationException("StringHandler can handle exactly one node");
            var node = nodes.First();
            if (node is not Attrib attrib)
                throw new InvalidOperationException("Only attribs can be handled by StringHandler");

            return actualTargetType.IsEncodingAwareString() ?
                 Activator.CreateInstance(actualTargetType, attrib.Content) :
                 options.DefaultEncoding.GetString(attrib.Content);
        }
    }
}
