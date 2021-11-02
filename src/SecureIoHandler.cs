using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 

namespace FileDBReader.src
{
    class SecureIoHandler
    {
        public static FileStream ReadHandle( String Filename )
        {
            try
            {
                return File.OpenRead(Filename);
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

        public static FileStream WriteHandle ( String Filename, bool overwrite)
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
            using (FileStream fs = WriteHandle(Filename, overwrite))
            {
                fs.Position = 0;
                Source.Position = 0;
                Source.CopyTo(fs);
                Source.Position = 0;
            }
        }
    }
}
