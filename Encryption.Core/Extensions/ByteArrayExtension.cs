using System.Collections.Generic;
using System.Linq;

namespace Encryption.Core.Extensions
{
    public static class ByteArrayExtension
    {
        public static IEnumerable<byte[]> GetBlocks(this byte[] array, int blockSize)
        {
            for (int i = 0; i < array.Length / blockSize; i++)
            {
                var block = array.Skip(i * blockSize).Take(blockSize).ToArray();
                yield return block;
            }
        }
    }
}
