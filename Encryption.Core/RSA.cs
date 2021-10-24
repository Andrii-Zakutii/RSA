using Encryption.Core.Extensions;
using System.Linq;
using System.Numerics;

namespace Encryption.Core
{
    public class RSA
    {
        private const byte RightByte = 0b11011101;
        private const int MessageBlockLength = 64;
        private const int EncryptedBlockLength = 128;

        public byte[] Encrypt(byte[] message, Key publicKey)
        {
            return message.ForEachBlock(block =>
            {
                return AddToLength(EncryptBlock(AddRightByte(block), publicKey));
            },
            MessageBlockLength);
        }

        public byte[] Decrypt(byte[] message, Key privateKey)
        {
            return message.ForEachBlock(block =>
            {
                return RemoveRightPadding(DecryptBlock(block, privateKey));
            },
            EncryptedBlockLength);
        }

        public byte[] AddToLength(byte[] block)
        {
            byte paddingValue;

            if (AsNumber(block).Sign == -1)
                paddingValue = 0b1111_1111;
            else
                paddingValue = 0b0000_0000;

            var padding = Enumerable.Repeat(paddingValue, EncryptedBlockLength - block.Length);
            return block.Concat(padding).ToArray();
        }

        public byte[] RemoveRightPadding(byte[] message) => message.Take(message.Length - 1).ToArray();

        public byte[] EncryptBlock(byte[] message, Key publicKey) => ModPow(message, publicKey.Exponent, publicKey.Module);

        public byte[] DecryptBlock(byte[] message, Key privateKey) => ModPow(message, privateKey.Exponent, privateKey.Module);

        private byte[] ModPow(byte[] message, byte[] exponent, byte[] module) =>
            BigInteger.ModPow(AsNumber(message), AsNumber(exponent), AsNumber(module)).ToByteArray();

        private byte[] AddRightByte(byte[] block) => block.Concat(OneBytePadding).ToArray();

        private byte[] OneBytePadding => new byte[] { RightByte };

        private BigInteger AsNumber(byte[] array) => new(array);
    }
}
