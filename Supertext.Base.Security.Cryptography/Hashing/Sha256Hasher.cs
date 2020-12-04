using System;
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
            var encoding = new UnicodeEncoding();
            var rawData = salt + apiToken + pepper;
            var bytes = encoding.GetBytes(rawData);
            var sha256 = new SHA256Managed();
            var hashedBytes = sha256.ComputeHash(bytes);
            

            return new HashingResult
                       {
                           HashedValue = Convert.ToBase64String(hashedBytes), 
                           Salt = salt
                       };
            
        }
    }
}