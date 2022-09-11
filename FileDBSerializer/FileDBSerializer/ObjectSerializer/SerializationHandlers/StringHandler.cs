using FileDBSerializing;
using FileDBSerializing.EncodingAwareStrings;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    /// <summary>
    /// Creates a FileDBNode that represents a single string object.
    /// </summary>
    public class StringHandler : ISerializationHandler
    {
        /// <summary>
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="property"></param>
        /// <param name="workingDocument"></param>
        /// <param name="options"></param>
        /// <returns>Attrib containing the string in the byte representation specified either by encodingawarestring or default encoding</returns>
        public FileDBNode Handle(object graph, PropertyInfo property, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            var objectInstance = property.GetValue(graph);
            Attrib attr = workingDocument.AddAttrib(property.Name);

            if (objectInstance is null)
            {
                attr.Content = new byte[0];
                return attr;
            }

            attr.Content = objectInstance is EncodingAwareString ? 
                ((EncodingAwareString)objectInstance).GetBytes()
                : options.DefaultEncoding.GetBytes((String)objectInstance!);
            return attr;
        }
    }
}
