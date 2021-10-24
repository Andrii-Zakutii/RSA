using System;
using System.Collections.Generic;
using System.Linq;

namespace Encryption.Core.Extensions
{
    public static class ByteArrayExtension
    {
        public static IEnumerable<byte[]> GetBlocks(this byte[] message, int blockSize)
        {
            for (int i = 0; i < message.Length / blockSize + 1; i++)
                yield return message.Skip(i * blockSize).Take(blockSize).ToArray();
        }

        public static byte[] ForEachBlock(this byte[] message, Func<byte[], byte[]> process, int blockSize)
        {
            List<byte> result = new();

            foreach (var block in message.GetBlocks(blockSize))
            {
                var encryptedBlock = process(block);
                result.AddRange(encryptedBlock);
            }

            return result.ToArray();
        }
    }
}
