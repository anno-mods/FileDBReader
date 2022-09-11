using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Reflection;

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

            Tag t = workingDocument.AddTag(property.Name);

            //Add array entries to the mix
            Type arrayContentType = property.GetNullablePropertyType().GetElementType()!;
            PropertyInfo[] contentProperties = arrayContentType.GetProperties();

            for (int i = 0; i < arrayInstance.Length; i++)
            {
                var arrayEntry = arrayInstance.GetValue(i);
                Tag none = workingDocument.AddTag(options.NoneTag);
                none.AddChildren(ToTags(arrayEntry!, contentProperties, workingDocument, options));
                t.AddChild(none);
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
