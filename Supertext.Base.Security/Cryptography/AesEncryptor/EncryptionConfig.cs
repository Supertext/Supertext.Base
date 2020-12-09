using Supertext.Base.Configuration;

namespace Supertext.Base.Security.Cryptography.AesEncryptor
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
