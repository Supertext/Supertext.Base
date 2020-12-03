using Supertext.Base.Configuration;

namespace Supertext.Base.Security.Cryptography.Hashing
{
    [ConfigSection("EncryptionSettings")]
    public class HashingConfig
    {
        [KeyVaultSecret("Legacy-Token-Hashing-Pepper")]
        public string LegacyTokenHashingPepper { get; set; }


        [KeyVaultSecret("Password-Hashing-Pepper")]
        public string PasswordHashingPepper { get; set; }
    }
}