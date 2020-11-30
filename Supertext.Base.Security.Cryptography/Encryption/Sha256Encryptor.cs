using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using NodaTime;

namespace Supertext.Base.Security.Cryptography.Encryption
{
    public class Sha256Encryptor : ISha256Encryptor
    {
        private readonly EncryptionConfig _encryptionConfig;

        public Sha256Encryptor(EncryptionConfig encryptionConfig)
        {
            _encryptionConfig = encryptionConfig;
        }

        public Tuple<string, string> HashWithSaltAndPepper(string entry)
        {
            var now = SystemClock.Instance.GetCurrentInstant();
            var timestampForSalt = now.InUtc().ToString("yyyy-MM-dd HH:mm:ss.fffffff",
                                                        CultureInfo.InvariantCulture);
            return new Tuple<string, string>(ComputeSha256HashWithSaltAndPepper(entry, timestampForSalt), timestampForSalt);
        }

        private string ComputeSha256HashWithSaltAndPepper(string apiToken, string salt)
        {
            // Create a SHA256   
            using (var sha256Hash = SHA256.Create())
            {
                var rawData = salt + apiToken + _encryptionConfig.Pepper;

                // ComputeHash - returns byte array with the hashed data 
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                var builder = new StringBuilder();
                foreach (var bt in bytes)
                {
                    builder.Append(bt.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}