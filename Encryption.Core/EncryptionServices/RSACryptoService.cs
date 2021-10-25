using System.Numerics;

namespace Encryption.Core.EncryptionServices
{
    public class RSACryptoService : ICryptoService
    {
        public Message Encrypt(Message plainText, Key key) =>
            plainText.ForEachBlock(block => block.Encrypt(this, key).AddToLength(128), blockLength: 64);

        public Message Decrypt(Message encryptedMessage, Key key) =>
            encryptedMessage.ForEachBlock(block => block.Decrypt(this, key), blockLength: 128);

        public Block Encrypt(Block block, Key key) => ModPow(block.AddRightByte(0b11011101), key);

        public Block Decrypt(Block block, Key key) => ModPow(block, key).RemoveRightByte();

        public Block ModPow(Block block, Key key)
        {
            var messageNumber = new BigInteger(block.Content);
            var exponentNumber = new BigInteger(key.Exponent);
            var moduleNumber = new BigInteger(key.Module);
            var result = BigInteger.ModPow(messageNumber, exponentNumber, moduleNumber);
            return new Block(result.ToByteArray());
        }
    }
}
