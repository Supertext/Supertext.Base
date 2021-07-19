using Autofac;
using Autofac.Core;
using Newtonsoft.Json;
using Supertext.Base.Common;
using Supertext.Base.Configuration;
using Supertext.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Supertext.Base.NetFramework.Configuration
{
    public static class ConfigurationExtension
    {
        public static void RegisterConfigurationsWithAppConfigValues(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            Validate.NotNull(assemblies, nameof(assemblies));

            RegisterAndInitializeConfigTypes(builder, assemblies);
        }

        private static void RegisterAndInitializeConfigTypes(ContainerBuilder builder, params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.GetTypeInfo().IsAssignableTo<IConfiguration>())
                .AsSelf()
                .OnActivating(SetupValues);
        }

        private static void SetupValues(IActivatingEventArgs<object> args)
        {
            var configInstance = args.Instance;
            var properties = configInstance.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                var settingKey = propertyInfo.GetCustomAttributes<SettingsKeyAttribute>().SingleOrDefault();
                if (settingKey != null)
                {
                    if (SetValueIfSome(propertyInfo, configInstance, settingKey.AppSettingsKey))
                    {
                        continue;
                    }
                }

                if (SettingConnectionStringWhenAvailable(propertyInfo, configInstance))
                {
                    continue;
                }

                var keyVaultKey = propertyInfo.GetCustomAttributes<KeyVaultSecretAttribute>().SingleOrDefault();
                if (keyVaultKey != null)
                {
                    if (SetValueIfSome(propertyInfo, configInstance, keyVaultKey.SecretName))
                    {
                        continue;
                    }
                }

                var jsonStructureKey = propertyInfo.GetCustomAttributes<JsonStructureKeyAttribute>().SingleOrDefault();
                if (jsonStructureKey != null)
                {
                    if (DeserializeAndSetValueIfSome(propertyInfo, configInstance, jsonStructureKey.AppSettingsKey))
                    {
                        continue;
                    }
                }

                var dictionaryPrefix = propertyInfo.GetCustomAttributes<DictionaryPrefixAttribute>().SingleOrDefault();
                if (dictionaryPrefix != null)
                {
                    if (CollectIntoDictionaryIfSome(propertyInfo, configInstance, dictionaryPrefix))
                    {
                        continue;
                    }
                }

                SetValueIfSome(propertyInfo, configInstance, propertyInfo.Name);
            }
        }

        private static bool SettingConnectionStringWhenAvailable(PropertyInfo propertyInfo, object configInstance)
        {
            var connectionStringAttribute = propertyInfo.GetCustomAttributes<ConnectionStringNameAttribute>().SingleOrDefault();
            if (connectionStringAttribute != null)
            {
                var connectionStringOption = GetConnectionStringValue(connectionStringAttribute.ConnectionStringName);
                if (connectionStringOption.IsSome)
                {
                    propertyInfo.SetValue(configInstance, Convert(connectionStringOption.Value, propertyInfo.PropertyType));
                    return true;
                }
            }

            return false;
        }

        private static bool DeserializeAndSetValueIfSome(PropertyInfo propertyInfo, object configInstance, string appsettingsKey)
        {
            var valueOption = GetSettingsValue(appsettingsKey);
            if (valueOption.IsSome)
            {
                var errorMessage = $"{propertyInfo.Name} has been decorated with {nameof(JsonStructureKeyAttribute)} and its type must therefore be implementation of Dictionary<string, T>.";

                if (!propertyInfo.PropertyType.IsGenericType || propertyInfo.PropertyType.GetGenericTypeDefinition() != typeof(Dictionary<,>))
                {
                    throw new ArgumentException(errorMessage);
                }

                var typeDictionary = propertyInfo.PropertyType;
                var dictionary = Activator.CreateInstance(typeDictionary);
                var genericsArgs = dictionary.GetType().GetGenericArguments();
                if (genericsArgs[0] != typeof(string))
                {
                    throw new ArgumentException(errorMessage);
                }

                var genericDictionary = typeof(Dictionary<,>);
                var constructedType = genericDictionary.MakeGenericType(genericsArgs);

                var deserializedValue = JsonConvert.DeserializeObject(valueOption.Value.ToString(), constructedType);
                propertyInfo.SetValue(configInstance, deserializedValue);
            }

            return valueOption.IsSome;
        }

        private static bool CollectIntoDictionaryIfSome(PropertyInfo propertyInfo, object instance, DictionaryPrefixAttribute dictionaryPrefixAttribute)
        {
            // Convert each value to match the target type.
            var errorMessage = $"{propertyInfo.Name} has been decorated with {nameof(DictionaryPrefixAttribute)} and its type must therefore be implementation of Dictionary<string, string>.";

            if (!propertyInfo.PropertyType.IsGenericType || propertyInfo.PropertyType.GetGenericTypeDefinition() != typeof(Dictionary<,>))
            {
                throw new ArgumentException(errorMessage);
            }

            var genericsArgs = propertyInfo.PropertyType.GetGenericArguments();
            if (genericsArgs[0] != typeof(string) || genericsArgs[1] != typeof(string))
            {
                throw new ArgumentException(errorMessage);
            }

            var optSettingsValues = GetSettingsValues(dictionaryPrefixAttribute.AppSettingPrefix);
            if (optSettingsValues.IsSome)
            {
                propertyInfo.SetValue(instance, optSettingsValues.Value);
            }

            return optSettingsValues.IsSome;
        }

        private static bool SetValueIfSome(PropertyInfo propertyInfo, object configInstance, string appsettingsKey)
        {
            var valueOption = GetSettingsValue(appsettingsKey);
            if (valueOption.IsSome)
            {
                propertyInfo.SetValue(configInstance, Convert(valueOption.Value, propertyInfo.PropertyType));
            }

            return valueOption.IsSome;
        }

        private static object Convert(object value, Type targetType)
        {
            var tc = TypeDescriptor.GetConverter(targetType);
            return tc.ConvertFrom(value);
        }

        private static Option<object> GetSettingsValue(string settingsKey)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == settingsKey))
            {
                var value = ConfigurationManager.AppSettings[settingsKey];
                return Option<object>.Some(value);
            }

            if (SupertextConfigurationManager.AppSettings.AllKeys.Any(key => key == settingsKey))
            {
                var value = SupertextConfigurationManager.AppSettings[settingsKey];
                return Option<object>.Some(value);
            }

            Console.WriteLine($"Key {settingsKey} not available");
            return Option<object>.None();
        }

        private static Option<Dictionary<string, string>> GetSettingsValues(string settingsPrefix)
        {
            Dictionary<string, string> AddFromNameValueCollection(NameValueCollection appSettings)
            {
                return appSettings.AllKeys.Where(key => key.StartsWith(settingsPrefix))
                                  .ToDictionary(keyWithPrefix => keyWithPrefix, keyWithPrefix => appSettings[keyWithPrefix]);
            }

            var applicableSettings = AddFromNameValueCollection(ConfigurationManager.AppSettings)
                                     .Concat(AddFromNameValueCollection(SupertextConfigurationManager.AppSettings))
                                     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            if (applicableSettings.None())
            {
                Console.WriteLine($"Key {settingsPrefix} not available");
                return Option<Dictionary<string, string>>.None();
            }

            return Option<Dictionary<string, string>>.Some(applicableSettings);
        }

        private static Option<object> GetConnectionStringValue(string connectionStringKey)
        {
            foreach (ConnectionStringSettings connectionStringSettings in ConfigurationManager.ConnectionStrings)
            {
                if (connectionStringSettings.Name.ToLowerInvariant() == connectionStringKey.ToLowerInvariant())
                {
                    var connStringSetting = ConfigurationManager.ConnectionStrings[connectionStringSettings.Name];
                    return Option<object>.Some(connStringSetting.ConnectionString);
                }
            }

            Console.WriteLine($"Connection string with name {connectionStringKey} not available");
            return Option<object>.None();
        }
    }
}