using System.Security.Cryptography;

namespace Encryption.Core.KeyGenerationServices
{
    public class StandardKeysGenerator : IKeysGenerator
    {
        public KeyPair CreateKeyPair()
        {
            var service = new RSACryptoServiceProvider();
            var parameters = service.ExportParameters(true);
            var publicKey = new Key(parameters.Exponent, parameters.Modulus);
            var privateKey = new Key(parameters.D, parameters.Modulus);
            return new KeyPair(publicKey, privateKey);
        }
    }
}
