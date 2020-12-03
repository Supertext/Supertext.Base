using Supertext.Base.Configuration;

namespace Supertext.Base.Security.Cryptography.Encryption
{
    [ConfigSection("EncryptionSettings")]
    public class EncryptionConfig
    {
        [KeyVaultSecret("Legacy-Token-Encryption-Pepper")]
        public string LegacyTokenEncryptionPepper { get; set; }
    }
}