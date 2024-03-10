using AnnoMods.BBDom;
using AnnoMods.BBDom.EncodingAwareStrings;
using AnnoMods.ObjectSerializer;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    /// <summary>
    /// A Handler for simple primitive values
    /// </summary>
    public class PrimitiveHandler : ISerializationHandler
    {

        public PrimitiveHandler()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="property"></param>
        /// <param name="workingDocument"></param>
        /// <param name="options"></param>
        /// <returns>An attrib containing a byte representation of the value</returns>
        public IEnumerable<BBNode> Handle(object? item, string tagName, IBBDocument workingDocument, BBSerializerOptions options)
        {
            if(item is null && options.SkipSimpleNullValues)
                return Enumerable.Empty<BBNode>();


            Attrib attr = workingDocument.CreateAttrib(tagName);

            attr.Content = item is null ?
                    new byte[0]
                    : PrimitiveTypeConverter.GetBytes(item);
            return attr.AsEnumerable();
        }
    }
}
