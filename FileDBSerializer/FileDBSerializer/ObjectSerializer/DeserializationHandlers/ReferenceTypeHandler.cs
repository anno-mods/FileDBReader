using FileDBSerializing;
using FileDBSerializing.LookUps;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.DeserializationHandlers
{
    public class ReferenceTypeHandler : IDeserializationHandler
    {
        public object? Handle(IEnumerable<FileDBNode> nodes, Type targetType, FileDBSerializerOptions options)
        {
            if (nodes.Count() != 1)
                throw new InvalidOperationException("ReferenceTypeHandler can handle exactly one node");
            var node = nodes.First();
            if (node is not Tag tag)
                throw new InvalidOperationException("Only Tags can be handled by ReferenceTypeHandler");
            var instance = Activator.CreateInstance(targetType);

            var names = tag.Children.Select(x => x.Name).Distinct();
            foreach (var name in names)
            {
                var children = tag.SelectNodes(name);

                //find the property corresponding to the child
                var propertyinfo = targetType.GetPropertyWithRenaming(name);
                if (propertyinfo is null && !options.IgnoreMissingProperties)
                    throw new InvalidProgramException($"{name} could not be resolved to a property of {targetType.Name}");
                else if (propertyinfo is null)
                    continue;

                var handler = HandlerProvider.GetHandlerFor(propertyinfo);
                var prop_instance = handler.Handle(children.AsEnumerable(), propertyinfo.PropertyType, options);
                propertyinfo.SetValue(instance, prop_instance);
            }

            return instance;
        }
    }
}
