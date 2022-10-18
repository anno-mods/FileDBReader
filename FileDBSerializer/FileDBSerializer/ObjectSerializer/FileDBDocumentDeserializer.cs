using FileDBSerializer.ObjectSerializer.DeserializationHandlers;
using FileDBSerializing.EncodingAwareStrings;
using FileDBSerializing.LookUps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FileDBSerializing.ObjectSerializer
{
    public class FileDBDocumentDeserializer<T> where T : class, new()
    {
        private FileDBSerializerOptions Options;
        private T TargetObject;
        private Type TargetType;

        public FileDBDocumentDeserializer(FileDBSerializerOptions options)
        {
            Options = options;
            TargetObject = new T();
            TargetType = TargetObject.GetType();
        }

        public T GetObjectStructureFromFileDBDocument(IFileDBDocument doc)
        {
            var targetType = typeof(T);

            PropertyInfo[] properties = targetType.GetProperties();
            var target = Activator.CreateInstance(targetType) as T;
            if (target is null)
                throw new InvalidOperationException($"Could not create an instance of {targetType}. A parameterless, public constructor is needed!");

            foreach (var property in properties)
            {
                var handler = HandlerProvider.GetHandlerFor(property);
                var nodes = doc.Roots.SelectNodes(property.Name);
                var obj = handler.Handle(nodes, property.PropertyType, Options);
                property.SetValue(target, obj);
            }

            return target;
        }
    }
}
