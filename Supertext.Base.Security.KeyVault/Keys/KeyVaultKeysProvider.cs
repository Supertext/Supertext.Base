using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;

namespace Supertext.Base.Security.KeyVault.Keys
{
    internal class KeyVaultKeysProvider : IKeyVaultKeysProvider
    {
        private readonly KeyVaultSettings _keyVaultSettings;
        private readonly IAzureKeyReader _azureKeyReader;

        public KeyVaultKeysProvider(KeyVaultSettings keyVaultSettings, IAzureKeyReader azureKeyReader)
        {
            _keyVaultSettings = keyVaultSettings;
            _azureKeyReader = azureKeyReader;
        }

        public async Task<string> GetRsaKeyAsync(string keyName)
        {
            var credential = new ClientSecretCredential(_keyVaultSettings.TenantId, _keyVaultSettings.AzureAdApplicationId, _keyVaultSettings.ClientSecret);
            var vaultUrl = $"https://{_keyVaultSettings.KeyVaultName}.vault.azure.net/";
            var keyClient = new KeyClient(new Uri(vaultUrl), credential);
            var key = await keyClient.GetKeyAsync(keyName).ConfigureAwait(false);
            var rsaKey = key.Value.Key.ToRSA();
            return _azureKeyReader.ReadPrivateKey(rsaKey);
        }
    }
}