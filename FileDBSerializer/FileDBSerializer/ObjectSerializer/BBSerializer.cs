using AnnoMods.BBDom;
using AnnoMods.BBDom.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AnnoMods.BBDom.ObjectSerializer
{
    /// <summary>
    /// Abstracts BBDoc Serialization into an IFormatter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BBSerializer<T> : IFormatter where T : class, new()
    {
        public SerializationBinder Binder { get; set; }
        public StreamingContext Context { get; set; }
        public ISurrogateSelector SurrogateSelector { get; set; }

        BBSerializerOptions Options;

        private Type _type = typeof(T);

        #region Constructor
        public BBSerializer(BBDocumentVersion version)
        {
            Options = new BBSerializerOptions();
            Options.Version = version;
        }

        public BBSerializer(BBSerializerOptions options)
        {
            Options = options;
        }
        #endregion

        //serializes an object into a BB document
        public void Serialize(Stream serializationStream, object graph)
        {
            BBDocumentSerializer builder = new BBDocumentSerializer(Options);
            var doc = builder.WriteObjectStructureToBBDocument(graph);
            BBDocumentWriter GenericSerializer = new BBDocumentWriter(Options.Version);
            GenericSerializer.WriteToStream(doc, serializationStream);
        }

        #region DESERIALIZING
        //serializes from a BB document into an object
        public object Deserialize(Stream serializationStream)
        {
            BBDocument? doc = null; 

            //autodetect version?
            var Version = VersionDetector.GetCompressionVersion(serializationStream);

            BBDocumentParser parser = new BBDocumentParser(Version);
            doc = parser.LoadBBDocument(serializationStream);
             
            BBDocumentDeserializer<T> deserializer = new BBDocumentDeserializer<T>(Options);            
            return deserializer.GetObjectStructureFromBBDocument(doc);
        }
        #endregion
    }
}
