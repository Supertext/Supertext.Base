using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Supertext.Base.Security.KeyVault
{
    /// <summary>
    /// Provider for accessing KeyVault keys.
    /// </summary>
    /// <remarks>
    /// Register <see cref="Supertext.Base.Security.KeyVault.SecurityKeyVaultModule"/> in bootstrap application.
    /// Register configuration <see cref="Supertext.Base.Security.KeyVault.KeyVaultSettings"/>.
    /// </remarks>
    public interface IKeyVaultKeysProvider
    {
        Task<RSA> GetRsaKeyAsync(string keyName);
    }
}