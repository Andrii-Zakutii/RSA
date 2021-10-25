using Encryption.Core.Extensions;
using System;
using System.Numerics;

namespace Encryption.Core.KeyGenerationServices
{
    public class CustomKeysGenerator : IKeysGenerator
    {
        private readonly Random _random = new Random();

        public KeyPair CreateKeyPair()
        {
            var p = _random.GetRandomPrimeNumber();
            var q = _random.GetRandomPrimeNumber();

            var n = p * q;
            var fi = (p - 1) * (q - 1);
            var e = GetE(fi);
            var d = GetD(e, fi);

            var publicKey = new Key(e.ToByteArray(), n.ToByteArray());
            var privateKey = new Key(d.ToByteArray(), n.ToByteArray());
            return new KeyPair(privateKey, publicKey);
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
            int sequenceNumber = 70;
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
