using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Configuration;
using Supertext.Base.Common;
using Supertext.Base.Configuration;
using IConfiguration = Supertext.Base.Configuration.IConfiguration;

namespace Supertext.Base.Core.Configuration.AzureFunctions
{
    public static class ConfigurationExtension
    {
        public static void RegisterConfigurationsWithAppConfigValues(this ContainerBuilder builder,
                                                                     Microsoft.Extensions.Configuration.IConfiguration configuration,
                                                                     params Assembly[] assemblies)
        {
            Validate.NotNull(assemblies, nameof(assemblies));
            RegisterAndInitializeConfigTypes(builder, configuration, assemblies);
        }

        private static void RegisterAndInitializeConfigTypes(ContainerBuilder builder, Microsoft.Extensions.Configuration.IConfiguration configuration, params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.GetTypeInfo().IsAssignableTo<IConfiguration>())
                .AsSelf()
                .OnActivating(args => SetupValues(args, configuration));
        }

        private static void SetupValues(IActivatingEventArgs<object> args, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var configInstance = args.Instance;
            var properties = configInstance.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                var settingKey = propertyInfo.GetCustomAttributes<SettingsKeyAttribute>().SingleOrDefault();
                if (settingKey != null)
                {
                    SetValueIfSome(propertyInfo, configInstance, settingKey.AppSettingsKey, configuration);
                    continue;
                }

                SetValueIfSome(propertyInfo, configInstance, propertyInfo.Name, configuration);
            }
        }

        private static void SetValueIfSome(PropertyInfo propertyInfo,
                                           object configInstance,
                                           string appsettingsKey,
                                           Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var valueOption = GetSettingsValue(appsettingsKey, configuration);
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

        private static Option<object> GetSettingsValue(string settingsKey, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var keyLower = settingsKey.ToLowerInvariant();
            if (configuration.AsEnumerable().Any(config => config.Key.ToLowerInvariant() == keyLower))
            {
                var value = configuration[settingsKey];
                return Option<object>.Some(value);
            }

            Console.WriteLine($"Key {settingsKey} not available");
            return Option<object>.None();
        }
    }
}