namespace Encryption.Core.KeyGenerationServices
{
    public interface IKeysGenerator
    {
        KeyPair CreateKeyPair();
    }
}
