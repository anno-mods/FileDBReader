using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public class ReferenceTypeHandler : ISerializationHandler
    {
        public IEnumerable<FileDBNode> Handle(object? item, string tagName, IFileDBDocument workingDocument, FileDBSerializerOptions options)
        {
            if (item is null && options.SkipSimpleNullValues)
                return Enumerable.Empty<FileDBNode>();

            //Get the instance of the property for our specific object as well as the properties of its type.

            Tag t = workingDocument.AddTag(tagName);

            if (item is null) return t.AsEnumerable();

            IEnumerable<PropertyInfo> properties = item.GetType().GetPropertiesWithOrder();

            foreach (var _prop in properties)
            {
                //use a handler for the children properties
                var handler = HandlerProvider.GetHandlerFor(_prop);
                var childItem = _prop.GetValue(item);
                t.AddChildren(handler.Handle(childItem, _prop.Name, workingDocument, options));
            }
            return t.AsEnumerable();
        }
    }
}
