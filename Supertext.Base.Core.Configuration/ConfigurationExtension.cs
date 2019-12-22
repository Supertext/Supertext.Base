using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Supertext.Base.Common;
using Supertext.Base.Configuration;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Supertext.Base.Core.Configuration
{
    public static class ConfigurationExtension
    {
        public static void RegisterConfigurationsWithAppConfigValues(this ContainerBuilder builder,
            IConfiguration configuration, params Assembly[] assemblies)
        {
            Validate.NotNull(configuration, nameof(configuration));
            Validate.NotNull(assemblies, nameof(assemblies));

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.GetTypeInfo().IsAssignableTo<Base.Configuration.IConfiguration>())
                .AsSelf()
                .OnActivating(setting => SettingActivating(setting, configuration));
        }

        private static void SettingActivating(IActivatingEventArgs<object> args, IConfiguration configuration)
        {
            var section = args.Instance.GetType().GetCustomAttribute<ConfigSectionAttribute>();
            if (section != null)
            {
                configuration.GetSection(section.SectionName).Bind(args.Instance);
            }

            SetKeyVaultSecrets(args, configuration);
        }

        private static void SetKeyVaultSecrets(IActivatingEventArgs<object> args, IConfiguration configuration)
        {
            var configInstance = args.Instance;
            var properties = configInstance.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                var keyVaultSecret = propertyInfo.GetCustomAttributes<KeyVaultSecretAttribute>().SingleOrDefault();
                if (keyVaultSecret != null)
                {
                    SetValueIfSome(propertyInfo, configInstance, configuration);
                }
            }
        }

        private static void SetValueIfSome(PropertyInfo propertyInfo, object configInstance, IConfiguration configuration)
        {
            var valueOption = GetSettingsValue(propertyInfo.Name, configuration);
            if (valueOption.IsSome)
            {
                propertyInfo.SetValue(configInstance, Convert(valueOption.Value, propertyInfo.PropertyType));
            }
        }

        private static object Convert(object value, Type targetType)
        {
            var tc = TypeDescriptor.GetConverter(targetType);
            return tc.ConvertFrom(value);
        }

        private static Option<string> GetSettingsValue(string settingsKey, IConfiguration configuration)
        {
            var value = configuration.GetValue<string>(settingsKey);
            if (value != null)
            {
                return Option<string>.Some(value);
            }

            Console.WriteLine($"Key {settingsKey} not available");
            return Option<string>.None();
        }
    }
}