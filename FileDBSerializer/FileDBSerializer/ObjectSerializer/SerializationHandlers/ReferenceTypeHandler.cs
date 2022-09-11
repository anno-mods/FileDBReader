using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System.Collections.Generic;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public class ReferenceTypeHandler : ISerializationHandler
    {
        public IEnumerable<FileDBNode> Handle(object graph, PropertyInfo property, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            //Get the instance of the property for our specific object as well as the properties of its type.
            var propertyInstance = property.GetValue(graph);
            Tag t = workingDocument.AddTag(property.Name);
            if (propertyInstance is null) return t.AsEnumerable();

            PropertyInfo[] properties = propertyInstance.GetType().GetProperties();

            foreach (var _prop in properties)
            {
                //use a handler for the children properties
                var handler = HandlerProvider.GetHandlerFor(_prop);
                t.AddChildren(handler.Handle(propertyInstance, _prop, workingDocument, options));
            }
            return t.AsEnumerable();
        }
    }
}
