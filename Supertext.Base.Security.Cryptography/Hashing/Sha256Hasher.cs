using System;
using System.Security.Cryptography;
using System.Text;
using Supertext.Base.Common;
using Supertext.Base.Extensions;
using Supertext.Base.Security.Cryptography.Hashing.Salt;
using Supertext.Base.Security.Hashing;

namespace Supertext.Base.Security.Cryptography.Hashing
{
    internal class Sha256Hasher : ISha256Hasher, ISha256InternalHasher
    {
        private readonly ISaltGenerator _saltGenerator;
        private readonly HashingConfig _hashingConfig;

        public Sha256Hasher(HashingConfig hashingConfig, ISaltGenerator saltGenerator)
        {
            _saltGenerator = saltGenerator;
            _hashingConfig = hashingConfig;
        }

        public HashingResult HashToken(string entry)
        {
            Validate.NotNullOrWhitespace(entry);

            return ComputeSha256HashWithSaltAndPepper(entry, _saltGenerator.Generate(), _hashingConfig.HashingPepperForToken);
        }

        public HashingResult HashPassword(string entry)
        {
            Validate.NotNullOrWhitespace(entry);

            return ComputeSha256HashWithSaltAndPepper(entry, _saltGenerator.Generate(), _hashingConfig.HashingPepperForPassword);
        }

        public HashingResult ComputeSha256HashWithSaltAndPepper(string rawData, string salt, string pepper)
        {
            var encoding = new UnicodeEncoding();
            var spicedRawData = salt + rawData + pepper;
            var bytes = encoding.GetBytes(spicedRawData);
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