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
    public class FileDBSerializer<T> : IFormatter where T : class, new()
    {
        public SerializationBinder Binder { get; set; }
        public StreamingContext Context { get; set; }
        public ISurrogateSelector SurrogateSelector { get; set; }

        FileDBSerializerOptions Options;

        private Type _type = typeof(T);

        #region Constructor
        public FileDBSerializer(BBDocumentVersion version)
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
            BBDocumentWriter GenericSerializer = new BBDocumentWriter(Options.Version);
            GenericSerializer.WriteToStream(doc, serializationStream);
        }

        #region DESERIALIZING
        //serializes from a filedb document into an object
        public object Deserialize(Stream serializationStream)
        {
            IBBDocument? doc = null; 

            //autodetect version?
            var Version = VersionDetector.GetCompressionVersion(serializationStream);

            BBDocumentParser parser = new BBDocumentParser(Version);
            doc = parser.LoadBBDocument(serializationStream);
             
            FileDBDocumentDeserializer<T> deserializer = new FileDBDocumentDeserializer<T>(Options);            
            return deserializer.GetObjectStructureFromFileDBDocument(doc);
        }
        #endregion
    }
}
