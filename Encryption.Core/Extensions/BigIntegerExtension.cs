using System;
using System.Numerics;

namespace Encryption.Core.Extensions
{
    public static class BigIntegerExtension
    {
        public static bool IsProbablyPrime(this BigInteger value, int witnesses = 10)
        {
            if (witnesses <= 0)
                throw new ArgumentException($"{nameof(witnesses)} must be > 0");

            if (value <= 1)
                return false;

            BigInteger d = value - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            byte[] bytes = new byte[value.ToByteArray().LongLength];
            BigInteger a;

            Random random = new Random();

            for (int i = 0; i < witnesses; i++)
            {
                do
                {
                    random.NextBytes(bytes);
                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= value - 2);

                BigInteger x = BigInteger.ModPow(a, d, value);

                if (x == 1 || x == value - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, value);

                    if (x == 1)
                        return false;

                    if (x == value - 1)
                        break;
                }

                if (x != value - 1)
                    return false;
            }

            return true;
        }
    }
}
