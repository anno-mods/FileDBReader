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
        public static Stream? ReadHandle( String Filename )
        {
            try
            {
                using (Stream stream = File.OpenRead(Filename))
                {
                    stream.Position = 0; 
                    MemoryStream fastStream = new MemoryStream();
                    stream.CopyTo(fastStream);
                    fastStream.Position = 0; 
                    return fastStream;
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Could not open File: {0} - File does not exist", Filename);
            }
            catch (IOException)
            {
                Console.WriteLine("Could not open File: {0} - File in Use or other unknown exception", Filename);
            }
            return null;
        }

        public static Stream? ReadHandleWithInterpreterRedirect(String Filename)
        {
            try
            {
                var execution_path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                if (!File.Exists(Filename) && !Path.IsPathRooted(Filename))
                {
                    var speculative_new_path = Path.Combine(execution_path, "FileFormats", Filename);
                    Console.WriteLine($"Could not find {Filename}. Redirecting to {speculative_new_path}");
                    return File.OpenRead(speculative_new_path);
                }

                return File.OpenRead(Filename);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Could not open File: {0} - File does not exist", Filename);
            }
            catch (IOException)
            {
                Console.WriteLine("Could not open File: {0} - File in Use or other unknown exception", Filename);
            }
            return null;
        }

        public static Stream? WriteHandle ( String Filename, bool overwrite)
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
                }
                catch (IOException)
                {
                    Console.WriteLine("Could not access File: {0} - File in Use or other unknown exception", Filename);
                }
                return null;
            }
        }
    }
}
