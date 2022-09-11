using FileDBSerializing;
using FileDBSerializing.EncodingAwareStrings;
using FileDBSerializing.ObjectSerializer;
using System.Collections.Generic;
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
        public IEnumerable<FileDBNode> Handle(object graph, PropertyInfo property, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            var PrimitiveObjectInstance = property.GetValue(graph);
            Attrib attr = workingDocument.AddAttrib(property.Name);

            attr.Content = PrimitiveObjectInstance is null ?
                    new byte[0]
                    : PrimitiveConverter!.GetBytes(PrimitiveObjectInstance);
            return attr.AsEnumerable();
        }
    }
}
