
namespace Supertext.Base.Security.Hashing
{
    public interface ISha256Hasher
    {
        HashingResult HashLegacyToken(string entry);

        HashingResult HashPassword(string entry);

        HashingResult ComputeSha256HashWithSaltAndPepper(string rawData, string salt, string pepper);
    }
}