
namespace Supertext.Base.Security.Hashing
{
    public interface ISha256HashValidator
    {
        bool IsLegacyTokenValid (string legacyToken, string salt, string hashedLegacyToken);

        bool IsPasswordValid(string password, string salt, string hashedLegacyToken);
    }
}