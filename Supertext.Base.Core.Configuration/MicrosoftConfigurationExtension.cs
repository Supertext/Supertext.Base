using System.Reflection;
using Microsoft.Extensions.Configuration;
using Supertext.Base.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Supertext.Base.Core.Configuration
{
    public static class MicrosoftConfigurationExtension
    {
        public static TSettings CreateConfiguredSettingsInstance<TSettings>(this IConfiguration configuration)
            where TSettings : Supertext.Base.Configuration.IConfiguration, new()
        {
            var settings = new TSettings();

            var section = settings.GetType().GetCustomAttribute<ConfigSectionAttribute>();
            if (section != null)
            {
                configuration.GetSection(section.SectionName).Bind(settings);
            }

            ConfigurationExtension.SetKeyVaultSecrets(settings, settings.GetType().GetProperties(), configuration);

            return settings;
        }
    }
}