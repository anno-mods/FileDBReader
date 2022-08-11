using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing
{
    public enum FileDBDocumentVersion { Version1 = 1, Version2 = 2, Version3 = 3 }

    public class Versioning
    {
        private static FileDBDocumentVersion[] Versions = 
            Enum.GetValues(typeof(FileDBDocumentVersion))
                .Cast<FileDBDocumentVersion>()
                .ToArray();

        public static void EnsureVersion(FileDBDocumentVersion version)
        {
            if (Versions.Contains(version))
                return;
            else
                throw new InvalidOperationException("Invalid FileDBVersion!!!");
        }

        public static byte[] GetMagicBytes(FileDBDocumentVersion version)
        {
            EnsureVersion(version);
            switch (version)
            { 
                case FileDBDocumentVersion.Version2: 
                    return new byte[] { 0x08, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0xFF, 0xFF };
                case FileDBDocumentVersion.Version3:
                    return new byte[] { 0x08, 0x00, 0x00, 0x00, 0xFD, 0xFF, 0xFF, 0xFF };
                default:
                    return new byte[0];
            }
        }
    }
}
