using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public class ReferenceArrayHandler : ISerializationHandler
    {
        public IEnumerable<FileDBNode> Handle(
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
                foreach (PropertyInfo _prop in contentProperties)
                {
                    var childnodes = HandlerProvider.GetHandlerFor(_prop).Handle(arrayEntry!, _prop, workingDocument, options);
                    none.AddChildren(childnodes);
                }
                t.AddChild(none);
            }
            return t.AsEnumerable();
        }
    }
}
