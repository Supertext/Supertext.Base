using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Supertext.Base.Common;
using Supertext.Base.Configuration;
using Microsoft.Extensions.Configuration;
using Supertext.Base.Authentication;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Supertext.Base.Core.Configuration
{
    public static class ConfigurationExtension
    {
        public static void RegisterIdentityAndApiResourceDefinitions(this ContainerBuilder builder,
                                                           IConfiguration configuration)
        {
            Validate.NotNull(configuration, nameof(configuration));

            builder.RegisterType<Identity>()
                   .AsSelf()
                   .OnActivating(setting =>
                                 {
                                     SettingActivating(setting, configuration);
                                     ExchangeClientSecrets(setting, configuration);
                                 });
        }

        public static void RegisterAllConfigurationsInAssembly(this ContainerBuilder builder,
                                                               IConfiguration configuration,
                                                               Assembly assembly)
        {
            Validate.NotNull(configuration, nameof(configuration));
            Validate.NotNull(assembly, nameof(assembly));

            builder.RegisterAssemblyTypes(assembly)
                   .Where(t => t.GetTypeInfo().IsAssignableTo<Base.Configuration.IConfiguration>())
                   .AsSelf()
                   .OnActivating(setting => SettingActivating(setting, configuration));
        }

        [Obsolete("Will soon be deprecated. Use RegisterAllConfigurationsInAssembly() instead.")]
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
                    SetValueIfSome(propertyInfo, configInstance, configuration, keyVaultSecret.SecretName ?? propertyInfo.Name);
                }
            }
        }

        private static void ExchangeClientSecrets(IActivatingEventArgs<object> args, IConfiguration configuration)
        {
            if (args.Instance is Identity identity)
            {
                foreach (var apiResourceDefinition in identity.ApiResourceDefinitions)
                {
                    var optionalClientSecret = GetSettingsValue(apiResourceDefinition.ClientSecretName, configuration);
                    if (optionalClientSecret.IsSome)
                    {
                        apiResourceDefinition.ClientSecret = optionalClientSecret.Value;
                    }
                }
            }
        }

        private static void SetValueIfSome(PropertyInfo propertyInfo,
                                           object configInstance,
                                           IConfiguration configuration,
                                           string settingsKey)
        {
            var valueOption = GetSettingsValue(settingsKey, configuration);
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