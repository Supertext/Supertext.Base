using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;

namespace Supertext.Base.Security.Configuration
{
    public static class KeyVaultExtensions
    {
        /// <summary>
        /// Adds Azure key vault to the configuration for environments Staging and Production.
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IHostBuilder ConfigureKeyVaultAppConfiguration(this IHostBuilder hostBuilder)
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
                                                         });
        }
    }
}