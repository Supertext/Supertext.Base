using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Supertext.Base.Security.Cryptography
{
    public class KeyVaultCertificateService : ICertificateService
    {
        private readonly string _vaultAddress;
        private readonly string _vaultClientId;
        private readonly string _vaultClientSecret;

        public KeyVaultCertificateService(string vaultAddress, string vaultClientId, string vaultClientSecret)
        {
            _vaultAddress = vaultAddress;
            _vaultClientId = vaultClientId;
            _vaultClientSecret = vaultClientSecret;
        }

        public X509Certificate2 GetCertificateFromKeyVault(string vaultCertificateName)
        {
            using (var keyVaultClient = new KeyVaultClient(AuthenticationCallback))
            {
                var certBundle = keyVaultClient.GetCertificateAsync(_vaultAddress, vaultCertificateName)
                                               .GetAwaiter()
                                               .GetResult();
                var certContent = keyVaultClient.GetSecretAsync(certBundle.SecretIdentifier.Identifier)
                                                .GetAwaiter()
                                                .GetResult();
                var certBytes = Convert.FromBase64String(certContent.Value);

                return new X509Certificate2(certBytes, String.Empty, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            }
        }

        private async Task<string> AuthenticationCallback(string authority, string resource, string scope)
        {
            var clientCredential = new ClientCredential(_vaultClientId, _vaultClientSecret);

            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, clientCredential);

            return result.AccessToken;
        }
    }
}