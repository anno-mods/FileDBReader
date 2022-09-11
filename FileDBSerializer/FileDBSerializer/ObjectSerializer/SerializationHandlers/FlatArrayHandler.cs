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
    internal class FlatArrayHandler : ISerializationHandler
    {
        public IEnumerable<FileDBNode> Handle(object graph, PropertyInfo property, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            var arrayInstance = property.GetValue(graph) as Array;
            if (arrayInstance is null) throw new InvalidOperationException($"{property.PropertyType} cannot be casted into an Array");

            Type arrayContentType = property.GetNullablePropertyType().GetElementType()!;
            PropertyInfo[] contentProperties = arrayContentType.GetProperties();

            for (int i = 0; i < arrayInstance.Length; i++)
            {
                var arrayEntry = arrayInstance.GetValue(i);
                Tag tag = workingDocument.AddTag(property.Name);

                foreach (PropertyInfo _prop in contentProperties)
                {
                    var childnodes = HandlerProvider.GetHandlerFor(_prop).Handle(arrayEntry!, _prop, workingDocument, options);
                    tag.AddChildren(childnodes);
                }
                yield return tag;
            }
        }
    }
}
