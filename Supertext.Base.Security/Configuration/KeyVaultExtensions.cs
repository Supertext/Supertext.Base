using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

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
                                                                 var clientId = vaultConfigSection["AzureADApplicationId"];
                                                                 var clientSecret = vaultConfigSection["ClientSecret"];

                                                                 using (var keyVaultClient = new KeyVaultClient((authority, resource, scope) =>
                                                                                                                    AuthenticationCallback(authority,
                                                                                                                                           resource,
                                                                                                                                           clientId,
                                                                                                                                           clientSecret
                                                                                                                                           )))
                                                                 {
                                                                     config.AddAzureKeyVault(vaultUrl,
                                                                                             keyVaultClient,
                                                                                             new DefaultKeyVaultSecretManager());
                                                                 }
                                                             }
                                                             else if (context.HostingEnvironment.IsDevelopment())
                                                             {
                                                                 config.AddUserSecrets<TStartup>();
                                                             }
                                                         });
        }

        private static async Task<string> AuthenticationCallback(string authority, string resource, string clientId, string clientSecret)
        {
            var clientCredential = new ClientCredential(clientId, clientSecret);

            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, clientCredential);

            return result.AccessToken;
        }
    }
}