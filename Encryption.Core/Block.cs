using System;
using System.Linq;
using System.Numerics;

namespace Encryption.Core
{
    public record Block
    {
        public const byte RightByte = 0b11011101;

        private readonly byte[] _content;

        public Block(byte[] content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            _content = content;
        }

        public byte[] Content => _content;

        public int Length => _content.Length;

        public Block AddRightByte() => new Block(_content.Append(RightByte).ToArray());

        public Block RemoveRightByte() => new Block(_content.Take(Length - 1).ToArray());

        public Block AddToLength(int length)
        {
            byte paddingValue = new BigInteger(_content).Sign == -1 ? (byte)255 : (byte)0;
            var padding = Enumerable.Repeat(paddingValue, length - _content.Length);
            return new Block(_content.Concat(padding).ToArray());
        }

        public Block Encrypt(Key key) => ModPow(key);

        public Block Decrypt(Key key) => ModPow(key);

        public Block ModPow(Key key)
        {
            var messageNumber = new BigInteger(_content);
            var exponentNumber = new BigInteger(key.Exponent);
            var moduleNumber = new BigInteger(key.Module);
            var result = BigInteger.ModPow(messageNumber, exponentNumber, moduleNumber);
            return new Block(result.ToByteArray());
        }
    }
}
