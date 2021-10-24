using System;
using System.Collections.Generic;
using System.Linq;

namespace Encryption.Core.Extensions
{
    public class Message
    {
        private readonly byte[] _content;

        public Message(byte[] content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            _content = content;
        }

        public int ByteLength => _content.Length;

        public Message Encrypt(Key key)
        {
            return ForEachBlock(e =>
            {
                return e.AddRightByte().Encrypt(key).AddToLength(128);
            },
            blockLength: 64);
        }

        public Message Decrpyt(Key key)
        {
            return ForEachBlock(e =>
            {
                return e.Decrypt(key).RemoveRightByte();
            },
            blockLength: 128);
        }

        private Message ForEachBlock(Func<Block, Block> process, int blockLength)
        {
            List<byte> result = new();

            foreach (var block in GetBlocks(blockLength))
                result.AddRange(process(block).Content);

            return new Message(result.ToArray());
        }

        private IEnumerable<Block> GetBlocks(int blockLength)
        {
            for (int i = 0; i < ByteLength / blockLength + 1; i++)
            {
                var blockContent = _content.Skip(i * blockLength).Take(blockLength).ToArray();
                yield return new Block(blockContent);
            }
        }
    }
}
