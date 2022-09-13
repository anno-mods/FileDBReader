using FileDBSerializing;
using FileDBSerializing.EncodingAwareStrings;
using FileDBSerializing.ObjectSerializer;
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
        private static PrimitiveTypeConverter? PrimitiveConverter;

        public PrimitiveHandler()
        {
            PrimitiveConverter ??= new PrimitiveTypeConverter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="property"></param>
        /// <param name="workingDocument"></param>
        /// <param name="options"></param>
        /// <returns>An attrib containing a byte representation of the value</returns>
        public IEnumerable<FileDBNode> Handle(object? item, string tagName, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            if(item is null && options.SkipSimpleNullValues)
                return Enumerable.Empty<FileDBNode>();


            Attrib attr = workingDocument.AddAttrib(tagName);

            attr.Content = item is null ?
                    new byte[0]
                    : PrimitiveConverter!.GetBytes(item);
            return attr.AsEnumerable();
        }
    }
}
