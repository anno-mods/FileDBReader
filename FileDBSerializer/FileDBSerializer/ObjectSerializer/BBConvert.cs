using AnnoMods.BBDom;
using System.Diagnostics;
using System.IO;

namespace AnnoMods.ObjectSerializer
{
    public static class BBConvert
    {
        /// <summary>
        /// Serializes an Object into a new Stream
        /// </summary>
        /// <typeparam name="SourceType"></typeparam>
        /// <param name="Source">Object of <typeparamref name="SourceType"/> to serialize</param>
        /// <param name="Options">Serializing Options</param>
        /// <returns>the Stream after serialization</returns>
        public static Stream SerializeObject<SourceType>(SourceType Source, BBSerializerOptions Options) where SourceType : class, new()
        {
            BBSerializer<SourceType> serializer = new BBSerializer<SourceType>(Options);
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
        public static Stream SerializeObject<SourceType>(SourceType Source, BBSerializerOptions Options, Stream SerializationStream) where SourceType : class, new()
        {
            BBSerializer<SourceType> serializer = new BBSerializer<SourceType>(Options);
            serializer.Serialize(SerializationStream, Source);
            return SerializationStream;
        }

        public static BBDocument SerializeObjectToDocument<SourceType>(SourceType Source, BBSerializerOptions Options) where SourceType : class, new()
        { 
            var serializer = new BBDocumentSerializer(Options);
            return serializer.WriteObjectStructureToBBDocument(Source);
        }

        /// <summary>
        /// Deserializes the SerializationStream into an object.
        /// </summary>
        /// <typeparam name="TargetType"></typeparam>
        /// <param name="SerializationStream"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public static TargetType? DeserializeObject<TargetType>(Stream SerializationStream, BBSerializerOptions Options) where TargetType : class, new()
        {
            BBSerializer<TargetType> serializer = new BBSerializer<TargetType>(Options);
            return serializer.Deserialize(SerializationStream) as TargetType;
        }

        public static TargetType? DeserializeObjectFromDocument<TargetType>(BBDocument document, BBSerializerOptions Options) where TargetType : class, new()
        {
            var deserializer = new BBDocumentDeserializer<TargetType>(Options);
            return deserializer.GetObjectStructureFromBBDocument(document);
        }


    }
}
