
namespace Supertext.Base.Security.Hashing
{
    /// <summary>
    /// This component can be used...
    ///
    /// Register Supertext.Base.Security.Cryptography.CryptographyModule with Autofac.
    /// .net Framework: 
    /// Configure properties of HashingConfig as secrets in Supertext appsettings.
    /// .net core:
    /// Configure properties of HashingConfig in appsettings.json
    /// </summary>
    public interface ISha256Hasher
    {
        HashingResult HashToken(string entry);

        HashingResult HashPassword(string entry);
    }
}