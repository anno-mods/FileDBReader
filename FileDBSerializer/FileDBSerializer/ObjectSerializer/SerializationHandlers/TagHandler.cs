using FileDBSerializer.ObjectSerializer.SerializationHandlerServices;
using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public class TagHandler : ISerializationHandler
    {
        public FileDBNode Handle(object graph, PropertyInfo property, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            //Get the instance of the property for our specific object as well as the properties of its type.
            var propertyInstance = property.GetValue(graph);
            Tag t = workingDocument.AddTag(property.Name);
            if (propertyInstance is null) return t;

            PropertyInfo[] properties = propertyInstance.GetType().GetProperties();

            foreach (var _prop in properties)
            {
                //use a handler for the children properties
                var handler = HandlerServices.TopLevelHandlerService.GetHandlerFor(_prop.PropertyType);
                t.AddChild(handler.Handle(propertyInstance, _prop, workingDocument, options));
            }
            return t;
        }
    }
}
