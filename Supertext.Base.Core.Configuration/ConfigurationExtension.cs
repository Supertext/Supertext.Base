using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Configuration;
using Supertext.Base.Common;
using Supertext.Base.Configuration;
using Supertext.Base.Extensions;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
            var customAttribute = args.Instance.GetType().GetCustomAttribute<ConfigSectionAttribute>();
            if (customAttribute != null)
            {
                var section = configuration.GetSection(customAttribute.SectionName);
                section.Bind(args.Instance);
            }
            else
            {
                configuration.Bind(args.Instance);
            }

            SetDictionaryValues(args.Instance, args.Instance.GetType().GetProperties(), configuration, customAttribute?.SectionName);

            SetKeyVaultSecrets(args.Instance, args.Instance.GetType().GetProperties(), configuration);
        }

        internal static void SetDictionaryValues(object configInstance,
                                                 PropertyInfo[] instancePropertyInfos,
                                                 IConfiguration configuration,
                                                 string sectionName = null)
        {
            if (configInstance == null)
            {
                return;
            }

            if (configInstance is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    SetDictionary(item, item.GetType().GetProperties(), configuration, sectionName);
                }
            }
            else
            {
                SetDictionary(configInstance, instancePropertyInfos, configuration, sectionName);
            }
        }

        private static void SetDictionary(object configInstance,
                                          PropertyInfo[] configInstancePropertyInfos,
                                          IConfiguration configuration,
                                          string sectionName = null)
        {
            foreach (var propertyInfo in configInstancePropertyInfos)
            {
                if (IsPrimitiveType(propertyInfo))
                {
                    SetDictionaryValue(configInstance, configuration, propertyInfo, sectionName);
                }
                else
                {
                    var enhancedSection = $"{sectionName}:{propertyInfo.Name}";
                    SetDictionaryValues(propertyInfo.GetValue(configInstance),
                                        propertyInfo.PropertyType.GetProperties(),
                                        configuration,
                                        enhancedSection);
                }
            }
        }

        internal static void SetKeyVaultSecrets(object configInstance, PropertyInfo[] instancePropertyInfos, IConfiguration configuration)
        {
            if (configInstance == null)
            {
                return;
            }

            if (configInstance is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    SetSecret(item, item.GetType().GetProperties(), configuration);
                }
            }
            else
            {
                SetSecret(configInstance, instancePropertyInfos, configuration);
            }
        }

        private static void SetSecret(object configInstance, PropertyInfo[] instancePropertyInfos, IConfiguration configuration)
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
            return type.IsPrimitive
                   || type == typeof(string)
                   || type == typeof(DateTime)
                   || type.IsDictionary()
                   || (type.IsArray
                       && (type.GetElementType()?.IsPrimitive == true
                           || type.GetElementType() == typeof(string)
                           || type.GetElementType() == typeof(DateTime)));
        }

        private static void SetDictionaryValue(object configInstance,
                                               IConfiguration configuration,
                                               PropertyInfo propertyInfo,
                                               string sectionName)
        {
            var attribute = propertyInfo.GetCustomAttributes<DictionaryPrefixAttribute>().SingleOrDefault();
            if (attribute == null)
            {
                return;
            }

            var section = configuration.GetSection(sectionName);
            var configParts = section.GetChildren()
                                     .Where(child => child.Key.StartsWith(attribute.AppSettingPrefix))
                                     .ToDictionary(part => part.Key, part => part.Value);

            propertyInfo.SetValue(configInstance, configParts);
        }
        
        private static void SetKeyVaultSecret(object configInstance, IConfiguration configuration, PropertyInfo propertyInfo)
        {
            var keyVaultSecret = propertyInfo.GetCustomAttributes<KeyVaultSecretAttribute>().SingleOrDefault();
            if (keyVaultSecret != null)
            {
                SetValueIfSome(propertyInfo,
                               configInstance,
                               configuration,
                               GetSecretName(configInstance, propertyInfo, keyVaultSecret));
            }
        }

        private static string GetSecretName(object configInstance, PropertyInfo propertyInfo, KeyVaultSecretAttribute keyVaultSecret)
        {
            if (keyVaultSecret.UsePropertyValueAsSecretName)
            {
                var propertyValue = propertyInfo.GetValue(configInstance)?.ToString();

                return !String.IsNullOrWhiteSpace(propertyValue) ? propertyValue : propertyInfo.Name;
            }

            return keyVaultSecret.SecretName ?? propertyInfo.Name;
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