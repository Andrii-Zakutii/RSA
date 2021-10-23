using Encryption.Core.Extensions;
using System.Numerics;
using Xunit;

namespace Encryption.Tests
{
    public class PrimaryNumbersTest
    {
        [Fact]
        public void Test()
        {
            BigInteger example = BigInteger.Parse("5317533587561651201917257772306211456582532650379866097970188384392018841328719023216255823814855817");
            Assert.True(example.IsProbablyPrime());
        }
    }
}
