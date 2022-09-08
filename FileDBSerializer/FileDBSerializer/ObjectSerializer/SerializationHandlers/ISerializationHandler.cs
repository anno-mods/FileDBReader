using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public interface ISerializationHandler
    {
        public FileDBNode Handle(object graph, PropertyInfo property, IFileDBDocument workingDocument, FileDBSerializerOptions options);
    }
}
