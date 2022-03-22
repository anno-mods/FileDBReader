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
    //simple combination that can 
    public class FileDBSerializer<T> : IFormatter where T : class, new()
    {
        public SerializationBinder Binder { get; set; }
        public StreamingContext Context { get; set; }
        public ISurrogateSelector SurrogateSelector { get; set; }

        FileDBSerializerOptions Options;

        private Type _type;

        #region Constructor
        public FileDBSerializer(FileDBDocumentVersion version)
        {
            _type = typeof(T);

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
