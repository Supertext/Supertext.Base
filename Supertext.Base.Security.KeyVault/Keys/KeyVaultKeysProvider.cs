using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;

namespace Supertext.Base.Security.KeyVault.Keys
{
    internal class KeyVaultKeysProvider : IKeyVaultKeysProvider
    {
        private readonly KeyVaultSettings _keyVaultSettings;

        public KeyVaultKeysProvider(KeyVaultSettings keyVaultSettings)
        {
            _keyVaultSettings = keyVaultSettings;
        }

        public async Task<string> GetRsaKeyAsync(string keyName)
        {
            var credential = new ClientSecretCredential(_keyVaultSettings.TenantId, _keyVaultSettings.AzureAdApplicationId, _keyVaultSettings.ClientSecret);
            var vaultUrl = $"https://{_keyVaultSettings.KeyVaultName}.vault.azure.net/";
            var keyClient = new KeyClient(new Uri(vaultUrl), credential);
            var key = await keyClient.GetKeyAsync(keyName).ConfigureAwait(false);
            return key.Value.Key.ToRSA().ToString();
        }
    }
}