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
        public static void RegisterIdentityAndApiResourceDefinitions(this ContainerBuilder builder,
                                                           IConfiguration configuration)
        {
            Validate.NotNull(configuration, nameof(configuration));

            builder.RegisterType<Authentication.Identity>()
                   .AsSelf()
                   .OnActivating(setting =>
                                 {
                                     SettingActivating(setting, configuration);
                                     ExchangeClientSecrets(setting, configuration);
                                 });
        }

        public static void RegisterAllConfigurationsInAssembly(this ContainerBuilder builder,
                                                               Assembly assembly)
        {
            Validate.NotNull(assembly, nameof(assembly));

            builder.RegisterAssemblyTypes(assembly)
                   .Where(t => t.GetTypeInfo().IsAssignableTo<Base.Configuration.IConfiguration>())
                   .AsSelf()
                   .OnActivating(setting => SettingActivating(setting));
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

        private static void SettingActivating(IActivatingEventArgs<object> args)
        {
            var configuration = args.Context.Resolve<IConfiguration>();
            SettingActivating(args, configuration);
        }

        private static void SettingActivating(IActivatingEventArgs<object> args, IConfiguration configuration)
        {
            var section = args.Instance.GetType().GetCustomAttribute<ConfigSectionAttribute>();
            if (section != null)
            {
                configuration.GetSection(section.SectionName).Bind(args.Instance);
            }
            else
            {
                configuration.Bind(args.Instance);
            }

            SetKeyVaultSecrets(args.Instance, args.Instance.GetType().GetProperties(), configuration);
        }

        internal static void SetKeyVaultSecrets(object configInstance, PropertyInfo[] instancePropertyInfos, IConfiguration configuration)
        {
            foreach (var propertyInfo in instancePropertyInfos)
            {
                if (IsPrimitiveType(propertyInfo))
                {
                    SetKeyVaultSecret(configInstance, configuration, propertyInfo);
                }
                else
                {
                    SetKeyVaultSecrets(propertyInfo.GetValue(configInstance),
                                       propertyInfo.PropertyType.GetProperties(),
                                       configuration);
                }
            }
        }

        private static bool IsPrimitiveType(PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;
            return type.IsPrimitive || type == typeof(string);
        }

        private static void SetKeyVaultSecret(object configInstance, IConfiguration configuration, PropertyInfo propertyInfo)
        {
            var keyVaultSecret = propertyInfo.GetCustomAttributes<KeyVaultSecretAttribute>().SingleOrDefault();
            if (keyVaultSecret != null)
            {
                SetValueIfSome(propertyInfo,
                               configInstance,
                               configuration,
                               keyVaultSecret.SecretName ?? propertyInfo.Name);
            }
        }

        private static void ExchangeClientSecrets(IActivatingEventArgs<object> args, IConfiguration configuration)
        {
            if (args.Instance is Authentication.Identity identity)
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