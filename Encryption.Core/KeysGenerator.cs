using Encryption.Core.Extensions;
using System;
using System.Numerics;

namespace Encryption.Core
{
    public class KeysGenerator
    {
        private readonly Random _random = new Random();

        public KeyPair GetRandomKeyPair()
        {
            BigInteger p = _random.GetRandomPrimeNumber();
            BigInteger q = _random.GetRandomPrimeNumber();

            var n = p * q;
            var fi = (p - 1) * (q - 1);
            var e = GetE(fi);
            var d = GetD(e, fi);

            var publicKey = new Key
            {
                Exponent = e.ToByteArray(),
                Module = n.ToByteArray()
            };

            var privateKey = new Key
            {
                Exponent = d.ToByteArray(),
                Module = n.ToByteArray()
            };

            return new KeyPair { PrivateKey = privateKey, PublicKey = publicKey };
        }

        public BigInteger GetD(BigInteger e, BigInteger module)
        {
            BigInteger i = module, v = 0, d = 1;
            while (e > 0)
            {
                BigInteger t = i / e, x = e;
                e = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }

            v %= module;

            if (v < 0)
                v = (v + module) % module;

            return v;
        }

        public BigInteger GetE(BigInteger fi)
        {
            int sequenceNumber = 7;
            int currentSequenceNumber = 0;

            for (int i = 2; i < fi; i++)
            {
                if (BigInteger.GreatestCommonDivisor(fi, i) == 1)
                {
                    currentSequenceNumber++;

                    if (currentSequenceNumber == sequenceNumber)
                        return i;
                }
            }

            throw new Exception("Not found");
        }
    }
}
