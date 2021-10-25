using Encryption.Core.EncryptionServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encryption.Core
{
    public record Message
    {
        private readonly byte[] _content;

        public Message(byte[] content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            _content = content;
        }

        public byte[] Content => _content;

        public int ByteLength => _content.Length;

        public Message Encrypt(ICryptoService service, Key key) => service.Encrypt(this, key);

        public Message Decrypt(ICryptoService service, Key key) => service.Decrypt(this, key);

        public override string ToString() => Encoding.ASCII.GetString(_content);

        public Message ForEachBlock(Func<Block, Block> process, int blockLength)
        {
            List<byte> result = new();

            foreach (var block in GetBlocks(blockLength))
                result.AddRange(process(block).Content);

            return new Message(result.ToArray());
        }

        private IEnumerable<Block> GetBlocks(int blockLength)
        {
            for (int i = 0; i < ByteLength / blockLength + 1; i++)
                yield return new Block(_content.Skip(i * blockLength).Take(blockLength).ToArray());
        }
    }
}
