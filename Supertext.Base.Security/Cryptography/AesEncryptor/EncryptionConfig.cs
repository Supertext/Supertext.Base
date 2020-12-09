using Supertext.Base.Configuration;

namespace Supertext.Base.Security.Cryptography.AesEncryptor
{
    [ConfigSection("Encryption")]
    public class EncryptionConfig : IConfiguration
    {
        public string Iv = "6c26%X2Viz@DvQq6";

        [KeyVaultSecret("Encryption-Key")]
        public string Key { get; set; }
    }
}
