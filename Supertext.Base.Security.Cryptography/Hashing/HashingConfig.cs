using Supertext.Base.Configuration;

namespace Supertext.Base.Security.Cryptography.Hashing
{
    [ConfigSection("EncryptionSettings")]
    public class HashingConfig
    {
        [KeyVaultSecret("Hashing-Pepper-For-Tokens")]
        public string HashingPepperForToken { get; set; }


        [KeyVaultSecret("Hashing-Pepper-For-Passwords")]
        public string HashingPepperForPassword { get; set; }
    }
}