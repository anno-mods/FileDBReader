using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoMods.BBDom
{
    public enum BBDocumentVersion { V1 = 1, V2 = 2, V3 = 3 }

    public class Versioning
    {
        private static BBDocumentVersion[] Versions =
            Enum.GetValues(typeof(BBDocumentVersion))
                .Cast<BBDocumentVersion>()
                .ToArray();

        public static void EnsureVersion(BBDocumentVersion version)
        {
            if (Versions.Contains(version))
                return;
            else
                throw new InvalidOperationException("Invalid FileDBVersion!!!");
        }

        public static byte[] GetMagicBytes(BBDocumentVersion version)
        {
            EnsureVersion(version);
            switch (version)
            {
                case BBDocumentVersion.V2:
                    return new byte[] { 0x08, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0xFF, 0xFF };
                case BBDocumentVersion.V3:
                    return new byte[] { 0x08, 0x00, 0x00, 0x00, 0xFD, 0xFF, 0xFF, 0xFF };
                default:
                    return new byte[0];
            }
        }

        public static int GetOffsetToOffsets(BBDocumentVersion version)
        {
            EnsureVersion(version);
            switch (version)
            {
                case BBDocumentVersion.V1:
                    return 4;
                default:
                    return 16;
            }
        }

        public static bool UsesAttribBlocks(BBDocumentVersion version)
        {
            EnsureVersion(version);
            return version != BBDocumentVersion.V1;
        }

        public static int GetBlockSize(BBDocumentVersion version)
        {
            EnsureVersion(version);
            switch (version)
            {
                case BBDocumentVersion.V1:
                    return 0;
                default:
                    return 8;
            }
        }
    }
}
