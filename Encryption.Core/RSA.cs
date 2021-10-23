using Encryption.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Encryption.Core
{
    /// <summary>
    /// Only for padded messages
    /// </summary>
    public class RSA
    {
        public const byte PaddingValue = 0b1101_1101;
        public const int MessageBlockLength = 64;
        public const int EncryptedBlockLength = 127;

        public byte[] Encrypt(byte[] message, Key publicKey, Key privateKey = null)
        {
            byte[] encryptedMessage = null;

            int unpaddedBlockSize = MessageBlockLength;
            var blocks = message.GetBlocks(unpaddedBlockSize);

            foreach (var block in blocks)
            {
                var encryptedBlock = AddPaddingThanEncrypt(block, publicKey);

                if (encryptedBlock.Length > EncryptedBlockLength)
                    throw new Exception($"{encryptedBlock.Length}");

                if (encryptedBlock.Length < EncryptedBlockLength)
                    encryptedBlock = encryptedBlock.Concat(new byte[EncryptedBlockLength - encryptedBlock.Length]).ToArray();

                if (privateKey != null)
                {
                    var dec = DecryptThanRemovePadding(encryptedBlock, privateKey);
                    Debug.Assert(dec.Length == block.Length);
                }

                if (encryptedMessage == null)
                    encryptedMessage = encryptedBlock;
                else
                    encryptedMessage = encryptedMessage.Concat(encryptedBlock).ToArray();
            }

            return encryptedMessage;
        }

        private byte[] AddDummyPadding(byte[] message, int bytesNumber)
        {
            BigInteger messageNumber = new BigInteger(message);
            byte paddingValue;

            if (messageNumber.Sign == -1)
                paddingValue = 0b1111_1111;
            else
                paddingValue = 0b0000_0000;

            var padding = Enumerable.Repeat(paddingValue, bytesNumber);
            return message.Concat(padding).ToArray();
        } 

        public byte[] Decrypt(byte[] message, Key privateKey)
        {
            byte[] decryptedMessage = null;

            var blocks = message.GetBlocks(128);

            foreach (var block in blocks)
            {
                var decryptedBlock = DecryptThanRemovePadding(block, privateKey);

                if (decryptedMessage == null)
                    decryptedMessage = decryptedBlock;
                else
                    decryptedMessage = decryptedMessage.Concat(decryptedBlock).ToArray();
            }

            return decryptedMessage;
        }

        public byte[] AddPaddingThanEncrypt(byte[] block, Key publicKey)
        {
            var paddedBlock = AddRightPadding(block);
            var encryptedBlock = EncryptBlock(paddedBlock, publicKey);
            return encryptedBlock;
        }

        public byte[] DecryptThanRemovePadding(byte[] block, Key privateKey)
        {
            var decryptedBlock = DecryptBlock(block, privateKey);
            var unpaddedBlock = decryptedBlock.Take(decryptedBlock.Length - 1).ToArray();
            return unpaddedBlock;
        }

        public byte[] EncryptBlock(byte[] message, Key publicKey) => ModPow(message, publicKey.Exponent, publicKey.Module);

        public byte[] DecryptBlock(byte[] message, Key privateKey) => ModPow(message, privateKey.Exponent, privateKey.Module);

        private byte[] ModPow(byte[] message, byte[] exponent, byte[] module)
        {
            var resultAsNumber = BigInteger.ModPow(AsNumber(message), AsNumber(exponent), AsNumber(module));
            return resultAsNumber.ToByteArray();
        }

        private int GetPaddedBlockLengthInBytes(byte[] module) => (int)Math.Floor((decimal)((module.Length * 8) - 1)) / 8;

        private byte[] RemovePaddings(byte[] block) => block.Skip(1).Take(block.Length - 2).ToArray();

        private byte[] AddPaddings(byte[] block) => AddLeftPadding(AddRightPadding(block));

        private byte[] AddRightPadding(byte[] block) => block.Concat(Padding).ToArray();

        private byte[] AddLeftPadding(byte[] block) => Padding.Concat(block).ToArray();

        private byte[] Padding => new byte[] { PaddingValue };

        private BigInteger AsNumber(byte[] array) => new BigInteger(array);
    }
}
