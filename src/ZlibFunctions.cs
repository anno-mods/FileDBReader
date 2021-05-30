using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBReader.src
{
    /// <summary>
    /// A simple deflate compression and decompression implementation, compressing Streams to byte arrays.
    /// </summary>
    public class ZlibFunctions
    {
        public ZlibFunctions() 
        {
        
        }

        /// <summary>
        /// Compresses a stream using zlib.
        /// </summary>
        /// <param name="stream">Original Stream</param>
        /// <param name="CompressionLevel">Compression level for zlib</param>
        /// <returns>zlib-compressed byte array</returns>
        public byte[] Compress(Stream stream, int CompressionLevel) {
            using var memoryStream = new MemoryStream();
            using var deflaterStream = new DeflaterOutputStream(memoryStream, new Deflater(CompressionLevel));

            //write input stream to the deflater stream 
            stream.Position = 0;
            stream.CopyTo(deflaterStream);
            deflaterStream.Close(); 

            return memoryStream.ToArray(); 
        }

        /// <summary>
        /// decompresses a zlib file
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>decompressed byte array</returns>
        public byte[] Decompress(Stream stream, int CompressionStartOffset) {
            using var decompressionStream = new InflaterInputStream(stream);
            using var decompressedFileStream = new MemoryStream();
            stream.Position = CompressionStartOffset;
            decompressionStream.CopyTo(decompressedFileStream);
            return decompressedFileStream.ToArray();
        }
    }
}
