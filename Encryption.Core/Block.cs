using Encryption.Core.EncryptionServices;
using System;
using System.Linq;
using System.Numerics;

namespace Encryption.Core
{
    public record Block
    {
        private readonly byte[] _content;

        public Block(byte[] content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            _content = content;
        }

        public byte[] Content => _content;

        public int Length => _content.Length;

        public Block Encrypt(ICryptoService service, Key key) => service.Encrypt(this, key);

        public Block Decrypt(ICryptoService service, Key key) => service.Decrypt(this, key);

        public Block AddRightByte(byte padding) => new Block(_content.Append(padding).ToArray());

        public Block RemoveRightByte() => new Block(_content.Take(Length - 1).ToArray());

        /// <summary>
        /// Adding bytes to the block to achieve the specified length 
        /// so that the numeric value of the block does not change
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public Block AddToLength(int length)
        {
            byte paddingValue = new BigInteger(_content).Sign == -1 ? (byte)0b11111111 : (byte)0;
            var padding = Enumerable.Repeat(paddingValue, length - _content.Length);
            return new Block(_content.Concat(padding).ToArray());
        }
    }
}
