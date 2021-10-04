using Supertext.Base.Security.Hashing;

namespace Supertext.Base.Security.Cryptography.Hashing
{
    internal interface ISha256InternalHasher
    {
        HashingResult ComputeSha256HashWithSaltAndPepper(string rawData, string salt, string pepper);
    }
}