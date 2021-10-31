using System.IO;

namespace FileDBSerializer
{
    public class Serializer
    {
        public static MemoryStream Serialize(FileDBDocument fileDBDocument)
        {
            FileDBSerializer serializer = new FileDBSerializer();
            return serializer.Serialize(fileDBDocument);
        }

        public static FileDBDocument Deserialize(Stream ms)
        {
            FileDBDeserializer deserializer = new FileDBDeserializer();
            return deserializer.Deserialize(ms);
        }

    }
}
