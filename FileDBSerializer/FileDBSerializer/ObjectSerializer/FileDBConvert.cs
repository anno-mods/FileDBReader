using System.IO;

namespace FileDBSerializing.ObjectSerializer
{
    public static class FileDBConvert
    {
        /// <summary>
        /// Serializes an Object into a new Stream
        /// </summary>
        /// <typeparam name="SourceType"></typeparam>
        /// <param name="Source">Object of <typeparamref name="SourceType"/> to serialize</param>
        /// <param name="Options">Serializing Options</param>
        /// <returns>the Stream after serialization</returns>
        public static Stream SerializeObject<SourceType>(SourceType Source, FileDBSerializerOptions Options) where SourceType : class, new()
        {
            FileDBSerializer<SourceType> serializer = new FileDBSerializer<SourceType>(Options);
            Stream stream = new MemoryStream();
            serializer.Serialize(stream, Source);
            return stream;
        }

        /// <summary>
        /// Serializes an Object into the provided Stream
        /// </summary>
        /// <typeparam name="SourceType"></typeparam>
        /// <param name="Source">Object of <typeparamref name="SourceType"/>to serialize</param>
        /// <param name="Options">Serializing Options</param>
        /// <param name="SerializationStream">Stream to serialize to</param>
        /// <returns>the Stream after serialization</returns>
        public static Stream SerializeObject<SourceType>(SourceType Source, FileDBSerializerOptions Options, Stream SerializationStream) where SourceType : class, new()
        {
            FileDBSerializer<SourceType> serializer = new FileDBSerializer<SourceType>(Options);
            serializer.Serialize(SerializationStream, Source);
            return SerializationStream;
        }

        /// <summary>
        /// Deserializes the SerializationStream into an object.
        /// </summary>
        /// <typeparam name="TargetType"></typeparam>
        /// <param name="SerializationStream"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public static TargetType? DeserializeObject<TargetType>(Stream SerializationStream, FileDBSerializerOptions Options) where TargetType : class, new()
        {
            FileDBSerializer<TargetType> serializer = new FileDBSerializer<TargetType>(Options);
            return serializer.Deserialize(SerializationStream) as TargetType;
        }
    }
}
