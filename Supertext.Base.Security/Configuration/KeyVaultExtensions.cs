﻿using System;
using System.Threading.Tasks;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
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
        ///     "ReadSecretsFromKeyVault": true,
        ///     "KeyVaultName": "kv-ne-dev",
        ///     "AzureADApplicationId": "456776-A386-4324-994A-3242344343",
        ///     "ClientSecret": "324234234-2222-4545-BF9F-df43534954",
        ///     "AzureADCertThumbprint": "456a7sad6f54fasdf787a9sdf6",
        ///     "CertificateName": "kv-dev-supertext-ch"
        /// },
        /// "IsUsingManagedIdentity": false
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IHostBuilder ConfigureKeyVaultAppConfiguration<TStartup>(this IHostBuilder hostBuilder) where TStartup : class
        {
            return hostBuilder.ConfigureAppConfiguration((context, config) =>
                                                         {
                                                             if (context.HostingEnvironment.IsStaging() || context.HostingEnvironment.IsProduction())
                                                             {
                                                                 config.ConfigureConfigWithKeyVaultSecrets();
                                                             }
                                                             else if (context.HostingEnvironment.IsDevelopment())
                                                             {
                                                                 config.AddUserSecrets<TStartup>();
                                                             }
                                                         });
        }

        /// <summary>
        /// Adds secrets from the key vault to the IConfigurationBuilder.
        ///
        /// Configuration in appsettings.json is mandatory as:
        /// "KeyVault": {
        ///     "ReadSecretsFromKeyVault": true,
        ///     "KeyVaultName": "kv-ne-dev",
        /// },
        /// "IsUsingManagedIdentity": true
        /// </summary>
        /// <param name="config"></param>
        public static void ConfigureConfigWithKeyVaultSecrets(this IConfigurationBuilder config)
        {
            var builtConfig = config.Build();
            var vaultConfigSection = builtConfig.GetSection("KeyVault");
            var vaultUrl = $"https://{vaultConfigSection["KeyVaultName"]}.vault.azure.net/";
            var isUsingManagedIdentity = builtConfig.GetValue<bool>("IsUsingManagedIdentity");

            if (isUsingManagedIdentity)
            {
                var secretClient = new SecretClient(new Uri(vaultUrl),
                                                    new DefaultAzureCredential());
                config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
            }
            else
            {
                var readSecretsFromKeyVault = vaultConfigSection.GetValue<bool>("ReadSecretsFromKeyVault");

                if (readSecretsFromKeyVault)
                {
                    var clientId = vaultConfigSection["AzureADApplicationId"];
                    var clientSecret = vaultConfigSection["ClientSecret"];

                    var keyVaultClient = new KeyVaultClient((authority, resource, scope) =>
                                                                AuthenticationCallback(authority,
                                                                                       resource,
                                                                                       clientId,
                                                                                       clientSecret));

                    config.AddAzureKeyVault(vaultUrl,
                                            keyVaultClient,
                                            new DefaultKeyVaultSecretManager());
                }
            }
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