using FileDBSerializing;
using FileDBSerializing.EncodingAwareStrings;
using FileDBSerializing.ObjectSerializer;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    /// <summary>
    /// A Handler for simple primitive values
    /// </summary>
    public class PrimitiveSingleValueHandler : ISerializationHandler
    {
        private static PrimitiveTypeConverter? PrimitiveConverter;

        public PrimitiveSingleValueHandler()
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
        public FileDBNode Handle(object graph, PropertyInfo property, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            var PrimitiveObjectInstance = property.GetValue(graph);
            Attrib attr = workingDocument.AddAttrib(property.Name);

            if (PrimitiveObjectInstance is null)
            {
                attr.Content = new byte[0];
                return attr;
            }
            attr.Content = PrimitiveConverter!.GetBytes(PrimitiveObjectInstance);
            return attr;
        }
    }
}
