using FileDBSerializing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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

    public class FileDBSerializer<T> : IFormatter where T : class, new()
    {
        public SerializationBinder Binder { get; set; }
        public StreamingContext Context { get; set; }
        public ISurrogateSelector SurrogateSelector { get; set; }

        FileDBSerializerOptions Options;

        private Type _type = typeof(T);

        #region Constructor
        public FileDBSerializer(FileDBDocumentVersion version)
        {
            Options = new FileDBSerializerOptions();
            Options.Version = version;
        }

        public FileDBSerializer(FileDBSerializerOptions options)
        {
            Options = options;
        }
        #endregion

        //serializes an object into a filedb document
        public void Serialize(Stream serializationStream, object graph)
        {
            FileDBDocumentSerializer builder = new FileDBDocumentSerializer(Options);
            var doc = builder.WriteObjectStructureToFileDBDocument(graph);
            DocumentWriter GenericSerializer = new DocumentWriter();
            GenericSerializer.WriteFileDBToStream(doc, serializationStream);
        }

        #region DESERIALIZING
        //serializes from a filedb document into an object
        public object Deserialize(Stream serializationStream)
        {
            IFileDBDocument? doc = null; 

            //autodetect version?
            var Version = VersionDetector.GetCompressionVersion(serializationStream);

            if (Version == FileDBDocumentVersion.Version1)
            {
                var parser = new DocumentParser<FileDBDocument_V1>();
                doc = parser.LoadFileDBDocument(serializationStream);
            }
            else if (Version == FileDBDocumentVersion.Version2)
            {
                var parser = new DocumentParser<FileDBDocument_V2>();
                doc = parser.LoadFileDBDocument(serializationStream);
            }

            if (doc is null) throw new Exception();
             
            FileDBDocumentDeserializer<T> deserializer = new FileDBDocumentDeserializer<T>(Options);            
            return deserializer.GetObjectStructureFromFileDBDocument(doc);
        }
        #endregion
    }
}
