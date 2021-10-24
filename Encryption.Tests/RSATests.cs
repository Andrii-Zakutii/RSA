using Encryption.Core;
using System;
using System.Linq;
using Xunit;

namespace Encryption.Tests
{
    public class RSATests
    {
        [Fact]
        public void Test3()
        {
            Random random = new Random();
            byte[] message = new byte[128 * 50 + 12];
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
        public void Test4()
        {
            byte[] l = new byte[1];
            l.Concat(Enumerable.Repeat((byte)10, 0));

        }
    }
}
