using Encryption.Core;
using System;
using Xunit;

namespace Encryption.Tests
{
    public class RSABlockTests
    {
        [Fact]
        public void OneByteTestOnce() => Test(GetRandomMessage(1));

        [Fact]
        public void OneByteTest() => Repeat(Test, GetRandomMessage(1), 100);

        [Fact]
        public void TwoByteTestOnce() => Test(GetRandomMessage(2));

        [Fact]
        public void TwoBytesTest() => Repeat(Test, GetRandomMessage(2), 100);

        [Fact]
        public void GeneralTestOnce() => Test(GetRandomMessage());

        [Fact]
        public void GeneralTest() => Repeat(Test, GetRandomMessage(), 10);

        private void Repeat(Action<byte[]> action, byte[] message, int times)
        {
            for (int i = 0; i < times; i++)
                action(message);
        }

        private void Test(byte[] message)
        {
            var keys = GetKeys();

            var decryptedMessage = EncryptThanDecrypt(message, keys);

            AssertEqualMessages(message, decryptedMessage);
        }

        private void AssertEqualMessages(byte[] message1, byte[] message2)
        {
            Assert.Equal(message1.Length, message2.Length);

            for (int i = 0; i < message1.Length; i++)
                Assert.Equal(message1, message2);
        }

        private byte[] GetRandomMessage()
        {
            Random random = new Random();
            int messageLength = random.Next(110, 130);
            var message = GetRandomMessage(messageLength);
            return message;
        }

        private byte[] GetRandomMessage(int length)
        {
            Random random = new Random();
            var message = new byte[length];
            random.NextBytes(message);
            return message;
        }

        private KeyPair GetKeys()
        {
            var keyGenerator = new KeysGenerator();
            var keys = keyGenerator.GetRandomKeyPair();
            return keys;
        }

        private byte[] EncryptThanDecrypt(byte[] message, KeyPair keys)
        {
            var rsa = new RSA();
            var encryptedMessage = rsa.DecryptBlock(message, keys.PublicKey);
            var decryptedMessage = rsa.DecryptBlock(encryptedMessage, keys.PrivateKey);
            return decryptedMessage;
        }
    }
}
