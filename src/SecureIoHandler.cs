using System;
using System.IO; 

namespace FileDBReader.src
{
    /// <summary>
    /// This class provides convenient handlers for accessing files via stream copying.
    /// The serializers do profit from MemoryStreams instead of FileStreams big time - up to 10x faster serialization
    /// </summary>
    class SecureIoHandler
    {
        public static Stream ReadHandle( String Filename )
        {
            try
            {
                using (Stream stream = File.OpenRead(Filename))
                {
                    stream.Position = 0; 
                    MemoryStream fastStream = new MemoryStream();
                    stream.CopyTo(fastStream);
                    return fastStream;
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Could not open File: {0} - File does not exist", Filename);
                throw new IOException();
            }
            catch (IOException)
            {
                Console.WriteLine("Could not open File: {0} - File in Use or other unknown exception", Filename);
                throw new IOException(); 
            }
        }

        public static Stream WriteHandle ( String Filename, bool overwrite)
        {
            if (File.Exists(Filename) && !overwrite)
            {
                Console.WriteLine("Could not access File: {0} - File already exists. use -y argument to overwrite.", Filename);
                throw new IOException();
            }
            else
            {
                try
                {
                    return File.Create(Filename);
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Could not access File: {0} - File does not exist", Filename);
                    throw new IOException();
                }
                catch (IOException)
                {
                    Console.WriteLine("Could not access File: {0} - File in Use or other unknown exception", Filename);
                    throw new IOException();
                }
            }
        }

        public static void SaveHandle(String Filename, bool overwrite, Stream Source)
        {
            using (Stream fs = WriteHandle(Filename, overwrite))
            {
                fs.Position = 0;
                Source.Position = 0;
                Source.CopyTo(fs);
                Source.Position = 0;
            }
        }
    }
}
