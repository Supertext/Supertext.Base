using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;

namespace Supertext.Base.Security.Configuration
{
    public static class KeyVaultExtensions
    {
        /// <summary>
        /// Adds Azure key vault to the configuration for environments Staging and Production.
        /// For Development environments it adds user secrets to the Startup class.
        ///
        /// Configuration in appsettings.json is mandatory as:
        /// "KeyVault": {
        ///     "KeyVaultName": "kv-ne-dev",
        ///     "AzureADApplicationId": "456776-A386-4324-994A-3242344343",
        ///     "ClientSecret": "324234234-2222-4545-BF9F-df43534954",
        ///     "AzureADCertThumbprint": "456a7sad6f54fasdf787a9sdf6",
        ///     "CertificateName": "kv-dev-supertext-ch"
        /// }
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IHostBuilder ConfigureKeyVaultAppConfiguration<TStartup>(this IHostBuilder hostBuilder) where TStartup : class
        {
            return hostBuilder.ConfigureAppConfiguration((context, config) =>
                                                         {
                                                             if (context.HostingEnvironment.IsStaging() || context.HostingEnvironment.IsProduction())
                                                             {
                                                                 var builtConfig = config.Build();
                                                                 var vaultConfigSection = builtConfig.GetSection("KeyVault");
                                                                 var vaultUrl = $"https://{vaultConfigSection["KeyVaultName"]}.vault.azure.net/";
                                                                 config.AddAzureKeyVault(vaultUrl,
                                                                                         vaultConfigSection["AzureADApplicationId"],
                                                                                         vaultConfigSection["ClientSecret"],
                                                                                         new DefaultKeyVaultSecretManager());
                                                             }
                                                             else if (context.HostingEnvironment.IsDevelopment())
                                                             {
                                                                 config.AddUserSecrets<TStartup>();
                                                             }
                                                         });
        }
    }
}