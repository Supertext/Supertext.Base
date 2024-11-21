using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;

namespace Supertext.Base.Security.Cryptography
{
    public class KeyVaultCertificateService : ICertificateService
    {
        private readonly string _tenantId;
        private readonly string _vaultAddress;
        private readonly string _vaultClientId;
        private readonly string _vaultClientSecret;

        public KeyVaultCertificateService(string tenantId, string vaultAddress, string vaultClientId, string vaultClientSecret)
        {
            _tenantId = tenantId;
            _vaultAddress = vaultAddress;
            _vaultClientId = vaultClientId;
            _vaultClientSecret = vaultClientSecret;
        }

        public X509Certificate2 GetCertificateFromKeyVault(string vaultCertificateName)
        {
            return RetrieveX509Certificate2(vaultCertificateName)
                   .GetAwaiter()
                   .GetResult();
        }

        private async Task<X509Certificate2> RetrieveX509Certificate2(string vaultCertificateName)
        {
            var credential = new ClientSecretCredential(_tenantId, _vaultClientId, _vaultClientSecret);
            var secretClient = new SecretClient(new Uri(_vaultAddress), credential);
            var certificateClient = new CertificateClient(new Uri(_vaultAddress), credential);

            var certBundle = await certificateClient.GetCertificateAsync(vaultCertificateName)
                                                    .ConfigureAwait(false);
            var identifier = new KeyVaultSecretIdentifier(certBundle.Value.SecretId);
            var secretResponse = await secretClient.GetSecretAsync(identifier.Name, identifier.Version);
            var secret = secretResponse.Value;
            var privateKeyBytes = Convert.FromBase64String(secret.Value);

            return X509CertificateLoader.LoadCertificate(privateKeyBytes);
        }
    }
}