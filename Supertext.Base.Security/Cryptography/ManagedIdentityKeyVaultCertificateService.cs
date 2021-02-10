using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Supertext.Base.Common;

namespace Supertext.Base.Security.Cryptography
{
    public class ManagedIdentityKeyVaultCertificateService
    {

        private readonly string _keyVaultEndpoint;
        private readonly string _certificateName;

        public ManagedIdentityKeyVaultCertificateService(string keyVaultEndpoint, string certificateName)
        {
            Validate.NotEmpty(keyVaultEndpoint, nameof(keyVaultEndpoint));
            Validate.NotEmpty(certificateName, nameof(certificateName));

            _keyVaultEndpoint = keyVaultEndpoint;
            _certificateName = certificateName;
        }

        public async Task<(X509Certificate2 ActiveCertificate, X509Certificate2 SecondaryCertificate)> GetCertificatesFromKeyVault()
        {
            var credential = new DefaultAzureCredential();
            var keyVaultUri = new Uri(_keyVaultEndpoint);

            var secretClient = new SecretClient(vaultUri: keyVaultUri, credential);

            var certificateClient = new CertificateClient(vaultUri: keyVaultUri, credential);

            (X509Certificate2 ActiveCertificate, X509Certificate2 SecondaryCertificate) certs = (null, null);

            var certificateItems = GetAllEnabledCertificateVersions(certificateClient);
            var item = certificateItems.FirstOrDefault();
            if (item != null)
            {
                certs.ActiveCertificate = await GetCertificateAsync(secretClient, _certificateName, item.Version);
            }

            if (certificateItems.Count > 1)
            {
                certs.SecondaryCertificate = await GetCertificateAsync(secretClient, _certificateName, certificateItems[1].Version);
            }

            return certs;
        }

        private List<CertificateProperties> GetAllEnabledCertificateVersions(CertificateClient certificateClient)
        {
            var certificateVersions = certificateClient.GetPropertiesOfCertificateVersions(_certificateName);

            return certificateVersions
                   .Where(certVersion => certVersion.Enabled.HasValue && certVersion.Enabled.Value)
                   .OrderByDescending(certVersion => certVersion.CreatedOn)
                   .ToList();
        }

        private async Task<X509Certificate2> GetCertificateAsync(SecretClient secretClient,
                                                                 string certName,
                                                                 string version)
        {
            var secretName = certName;
            KeyVaultSecret secret = await secretClient.GetSecretAsync(secretName, version);

            var privateKeyBytes = Convert.FromBase64String(secret.Value);

            return new X509Certificate2(privateKeyBytes,
                                        (string) null,
                                        X509KeyStorageFlags.MachineKeySet);
        }
    }
}