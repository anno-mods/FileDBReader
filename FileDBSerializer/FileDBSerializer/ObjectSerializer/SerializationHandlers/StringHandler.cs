using FileDBSerializing;
using FileDBSerializing.EncodingAwareStrings;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<FileDBNode> Handle(object? item, string tagName, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            if (item is null && options.SkipSimpleNullValues)
                return Enumerable.Empty<FileDBNode>();

            Attrib attr = workingDocument.AddAttrib(tagName);

            if (item is null)
            {
                attr.Content = new byte[0];
            }
            else
            {
                attr.Content = item is EncodingAwareString ?
                    ((EncodingAwareString)item).GetBytes()
                    : options.DefaultEncoding.GetBytes((String)item!);
            }

            return attr.AsEnumerable();
        }
    }
}
