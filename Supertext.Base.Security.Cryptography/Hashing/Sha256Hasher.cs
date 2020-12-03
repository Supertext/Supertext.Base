using System.Security.Cryptography;
using System.Text;
using Supertext.Base.Security.Cryptography.Hashing.Salt;
using Supertext.Base.Security.Hashing;

namespace Supertext.Base.Security.Cryptography.Hashing
{
    internal class Sha256Hasher : ISha256Hasher
    {
        private readonly ISaltGenerator _saltGenerator;
        private readonly HashingConfig _hashingConfig;

        public Sha256Hasher(ISaltGenerator saltGenerator, HashingConfig hashingConfig)
        {
            _saltGenerator = saltGenerator;
            _hashingConfig = hashingConfig;
        }

        public HashingResult HashLegacyToken(string entry)
        {
            return ComputeSha256HashWithSaltAndPepper(entry, _saltGenerator.Generate(), _hashingConfig.LegacyTokenHashingPepper);
        }

        public HashingResult HashPassword(string entry)
        {
            return ComputeSha256HashWithSaltAndPepper(entry, _saltGenerator.Generate(), _hashingConfig.PasswordHashingPepper);
        }

        private HashingResult ComputeSha256HashWithSaltAndPepper(string rawData, string salt, string pepper)
        {
            // Create a SHA256   
            using (var sha256Hash = SHA256.Create())
            {
                var spicedRawData = salt + rawData + pepper;

                // ComputeHash - returns byte array with the hashed data 
                var hashedBytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(spicedRawData));

                // Convert byte array to a string   
                var builder = new StringBuilder();
                foreach (var bt in hashedBytes)
                {
                    builder.Append(bt.ToString("x2"));
                }

                return new HashingResult
                       {
                           HashedValue = builder.ToString(), 
                           Salt = salt
                       };
            }
        }
    }
}