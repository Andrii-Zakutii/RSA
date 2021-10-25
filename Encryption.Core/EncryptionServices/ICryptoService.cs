namespace Encryption.Core.EncryptionServices
{
    public interface ICryptoService
    {
        Message Encrypt(Message palinText, Key key);
        Message Decrypt(Message encrypteMessage, Key key);
        Block Encrypt(Block block, Key key);
        Block Decrypt(Block blokc, Key key);
    }
}
