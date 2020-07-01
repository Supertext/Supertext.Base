using Supertext.Base.Configuration;

namespace Supertext.Base.Security.KeyVault
{
    [ConfigSection("KeyVault")]
    public class KeyVaultSettings : IConfiguration
    {
        public string AzureAdApplicationId { get; set; }

        public string ClientSecret { get; set; }

        public string KeyVaultName { get; set; }

        public bool ReadSecretsFromKeyVault { get; set; }

        public string TenantId { get; set; }
    }
}