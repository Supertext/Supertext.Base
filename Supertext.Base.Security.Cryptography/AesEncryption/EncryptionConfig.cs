using Supertext.Base.Configuration;

namespace Supertext.Base.Security.Cryptography.AesEncryption
{
    [ConfigSection("Encryption")]
    public class EncryptionConfig : IConfiguration
    {
        [KeyVaultSecret("Encryption-Iv")]
        public string Iv { get; set; }

        [KeyVaultSecret("Encryption-Key")]
        public string Key { get; set; }
    }
}
