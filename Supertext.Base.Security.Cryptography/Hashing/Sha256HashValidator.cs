using System;
using Supertext.Base.Security.Hashing;

namespace Supertext.Base.Security.Cryptography.Hashing
{
    internal class Sha256HashValidator : ISha256HashValidator
    {
        private readonly HashingConfig _hashingConfig;
        private readonly ISha256InternalHasher _sha256Hasher;

        public Sha256HashValidator(HashingConfig hashingConfig, ISha256InternalHasher sha256Hasher)
        {
            _hashingConfig = hashingConfig;
            _sha256Hasher = sha256Hasher;
        }

        public bool IsTokenValid(string token, string salt, string hashedToken)
        {
            return IsHashValid(token, salt, _hashingConfig.HashingPepperForToken, hashedToken);
        }

        public bool IsPasswordValid(string password, string salt, string hashedPassword)
        {
            return IsHashValid(password, salt, _hashingConfig.HashingPepperForPassword, hashedPassword);
        }

        private bool IsHashValid(string rawData, string salt, string pepper, string hashedData)
        {
            var hashingResult = _sha256Hasher.ComputeSha256HashWithSaltAndPepper(rawData, salt, pepper);
            return String.Equals(hashedData, hashingResult.HashedValue);
        }
    }
}