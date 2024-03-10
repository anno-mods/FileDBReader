using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public class PrimitiveArrayHandler : ISerializationHandler
    {

        public PrimitiveArrayHandler()
        {
        }

        public IEnumerable<BBNode> Handle(object? item, string tagName, IBBDocument workingDocument, FileDBSerializerOptions options)
        {
            var arrayInstance = item as Array;
            if (arrayInstance is null && options.SkipSimpleNullValues)
                return Enumerable.Empty<BBNode>();

            Attrib attr = workingDocument.CreateAttrib(tagName);

            if (arrayInstance is null)
            {
                attr.Content = new byte[0];
                return attr.AsEnumerable();
            }

            //builds a byte array out of Array Content
            using (MemoryStream ContentStream = new MemoryStream())
            {
                for (int i = 0; i < arrayInstance.Length; i++)
                {
                    var singleVal = arrayInstance.GetValue(i);
                    //can singleVal be null here? idk, may lead to errors.
                    ContentStream.Write(PrimitiveTypeConverter.GetBytes(singleVal));
                }
                attr.Content = ContentStream.ToArray();
            }
            return attr.AsEnumerable();
        }
    }
}
