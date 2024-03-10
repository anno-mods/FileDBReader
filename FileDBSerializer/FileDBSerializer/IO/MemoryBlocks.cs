using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoMods.BBDom.IO
{
    internal class MemoryBlocks
    {
        /// <summary>
        /// Computes the total space needed to represent <paramref name="bytesize"/> in a block of n * <paramref name="block_size"/>.
        /// </summary>
        /// <param name="bytesize"></param>
        /// <param name="block_size"></param>
        /// <returns></returns>
        public static int GetBlockSpace(int bytesize, int block_size)
        {
            return bytesize / block_size * block_size + block_size * Math.Clamp(bytesize % block_size, 0, 1);
        }

    }
}
