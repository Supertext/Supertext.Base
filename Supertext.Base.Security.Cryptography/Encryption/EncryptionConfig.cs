using Supertext.Base.Configuration;

namespace Supertext.Base.Security.Cryptography.Encryption
{
    [ConfigSection("EncryptionSettings")]
    public class EncryptionConfig
    {
        [KeyVaultSecret]
        public string Pepper { get; set; }
    }
}