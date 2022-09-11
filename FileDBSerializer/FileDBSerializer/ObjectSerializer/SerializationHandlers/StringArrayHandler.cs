using FileDBSerializing;
using FileDBSerializing.EncodingAwareStrings;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public class StringArrayHandler : ISerializationHandler
    {
        public IEnumerable<FileDBNode> Handle(object graph, PropertyInfo property, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            var arrayInstance = property.GetValue(graph) as Array;
            if (arrayInstance is null) throw new InvalidOperationException($"{property.PropertyType} cannot be casted into an Array");

            Tag t = workingDocument.AddTag(property.Name);

            for (int i = 0; i < arrayInstance.Length; i++)
            {
                var arrayEntry = arrayInstance.GetValue(i);
                Attrib none = workingDocument.AddAttrib(options.NoneTag);

                none.Content = arrayEntry is null ? new byte[0] :
                    arrayEntry is EncodingAwareString ?
                    ((EncodingAwareString)arrayEntry).GetBytes()
                    : options.DefaultEncoding.GetBytes((String)arrayEntry!);
                t.AddChild(none);
            }
            return t.AsEnumerable();
        }
    }
}
