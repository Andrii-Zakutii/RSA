using Encryption.Core;
using System;
using System.Linq;
using System.Numerics;
using Xunit;

namespace Encryption.Tests
{
    public class RSATests
    {
        [Fact]
        public void OneEncryptedBlockLengthTest()
        {
            // var oneBlockMessage = new byte[] { 0x55, 0x55, 0x55, 0x55, 0x55, 0x55 };

            Random random = new Random();
            var oneBlockMessage = new byte[120];
            random.NextBytes(oneBlockMessage);

            var keyGenerator = new KeysGenerator();
            var keys = keyGenerator.GetRandomKeyPair();

            var rsa = new RSA();

            var encrpyted = rsa.EncryptBlock(oneBlockMessage, keys.PublicKey);
            var decrpyted = rsa.DecryptBlock(encrpyted, keys.PrivateKey);

            Assert.Equal(oneBlockMessage.Length, decrpyted.Length);

            for (int i = 0; i < oneBlockMessage.Length; i++)
                Assert.Equal(oneBlockMessage[i], decrpyted[i]);

            var encrypted2 = rsa.AddPaddingThanEncrypt(oneBlockMessage, keys.PublicKey);
            var decrypted2 = rsa.DecryptThanRemovePadding(encrypted2, keys.PrivateKey);

            Assert.Equal(oneBlockMessage.Length, decrypted2.Length);

            for (int i = 0; i < oneBlockMessage.Length; i++)
                Assert.Equal(oneBlockMessage[i], decrypted2[i]);
        }

        [Fact]
        public void Test2()
        {
            for (int i = 0; i < 100; i++)
            {
                Random random = new Random();
                byte[] message = { 0x56, 0x1A, 0x00, 0x00, 0x00 };
                random.NextBytes(message);

                var keyGenerator = new KeysGenerator();
                var keys = keyGenerator.GetRandomKeyPair();

                var rsa = new RSA();

                var encrpyted = rsa.AddPaddingThanEncrypt(message, keys.PublicKey);
                var decrpyted = rsa.DecryptThanRemovePadding(encrpyted, keys.PrivateKey);

                Assert.Equal(message.Length, decrpyted.Length);

                for (int j = 0; j < message.Length; j++)
                    Assert.Equal(message[j], decrpyted[j]);
            }

        }

        [Fact]
        public void Test3()
        {
            Random random = new Random();
            byte[] message = new byte[128];
            random.NextBytes(message);

            var keyGenerator = new KeysGenerator();
            var keys = keyGenerator.GetRandomKeyPair();

            var rsa = new RSA();

            var encrpyted = rsa.Encrypt(message, keys.PublicKey);
            var decrpyted = rsa.Decrypt(encrpyted, keys.PrivateKey);

            Assert.Equal(message.Length, decrpyted.Length);

            for (int j = 0; j < message.Length; j++)
                Assert.Equal(message[j], decrpyted[j]);
        }

        [Fact]
        public void Test31()
        {
            Random random = new Random();
            byte[] message = new byte[128];
            random.NextBytes(message);

            var keyGenerator = new KeysGenerator();
            var keys = keyGenerator.GetRandomKeyPair();

            var rsa = new RSA();

            var encrpyted = rsa.Encrypt(message, keys.PublicKey, keys.PrivateKey);
            var decrypted = rsa.Decrypt(encrpyted, keys.PrivateKey);

            Assert.Equal(256, encrpyted.Length);
            Assert.Equal(128, decrypted.Length);
        }

        [Fact]
        public void Test4()
        {
            Random random = new Random();

            byte[] message1 = { 5, 0, 0, 0 };

            byte[] message2 = { 5 };

            message2 = message2.Concat(new byte[5]).ToArray();

            var keyGenerator = new KeysGenerator();
            var keys = keyGenerator.GetRandomKeyPair();

            var rsa = new RSA();

            var encrpyted1 = rsa.EncryptBlock(message1, keys.PublicKey);
            var encrpyted2 = rsa.EncryptBlock(message2, keys.PublicKey);

            Assert.Equal(encrpyted1.Length, encrpyted2.Length);
        }

        [Fact]
        public void Test5()
        {
            Random random = new Random();
            var message = new byte[64];
            random.NextBytes(message);

            var keyGenerator = new KeysGenerator();
            var keys = keyGenerator.GetRandomKeyPair();

            var rsa = new RSA();

            var encrypted = rsa.AddPaddingThanEncrypt(message, keys.PublicKey);

            encrypted = (new byte[1]).Concat(encrypted).ToArray();

            var decrypted = rsa.DecryptThanRemovePadding(encrypted, keys.PrivateKey);

            Assert.Equal(message.Length, decrypted.Length);

            for (int j = 0; j < message.Length; j++)
                Assert.Equal(message[j], decrypted[j]);
        }

        [Fact]
        public void Test100500()
        {
            for (int i = 0; i < 100; i++)
            {
                Random random = new Random();
                var message = new byte[127];
                random.NextBytes(message);

                BigInteger i1 = new BigInteger(message);
                byte[] message2;

                if (new BigInteger(message).Sign == -1)
                    message2 = message.Concat(new byte[] { 0b1111_1111 }).ToArray();
                else
                    message2 = message.Concat(new byte[] { 0b0000_0000 }).ToArray();

                BigInteger i2 = new BigInteger(message2);

                Assert.Equal(i1, i2);
            }
        }
    }
}
