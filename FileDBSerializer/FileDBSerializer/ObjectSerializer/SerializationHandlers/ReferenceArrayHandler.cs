using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public class ReferenceArrayHandler : ISerializationHandler
    {
        public FileDBNode Handle(
            object graph, 
            PropertyInfo property, 
            IFileDBDocument workingDocument,
            FileDBSerializerOptions options)
        {
            var arrayInstance = property.GetValue(graph) as Array;
            if (arrayInstance is null) throw new InvalidOperationException($"{property.PropertyType} cannot be casted into an Array");
            
            Type arrayContentType = property.GetNullablePropertyType().GetElementType()!;
            PropertyInfo[] contentProperties = arrayContentType.GetProperties();

            Tag t = workingDocument.AddTag(options.NoneTag);
            for (int i = 0; i < arrayInstance.Length; i++)
            {
                var arrayEntry = arrayInstance.GetValue(i);
                t.AddChildren(ToTags(arrayEntry!, contentProperties, workingDocument, options));
            }
            return t;
        }

        private IEnumerable<FileDBNode> ToTags(
            object graph, 
            PropertyInfo[] properties, 
            IFileDBDocument workingDocument,
            FileDBSerializerOptions options
        )
        {
            foreach (var _prop in properties)
            {
                var handler = HandlerProvider.GetHandlerFor(_prop);
                yield return handler.Handle(graph, _prop, workingDocument, options);
            }
        }
    }
}
