using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Supertext.Base.Common;
using Supertext.Base.Configuration;

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
                .OnActivated(SetupValues);
        }

        private static void SetupValues(IActivatedEventArgs<object> args)
        {
            var configInstance = args.Instance;
            var properties = configInstance.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                var settingKey = propertyInfo.GetCustomAttributes<SettingsKeyAttribute>().SingleOrDefault();
                if (settingKey != null)
                {
                    SetValueIfSome(propertyInfo, configInstance, settingKey.AppSettingsKey);
                    continue;
                }

                SetValueIfSome(propertyInfo, configInstance, propertyInfo.Name);
            }
        }

        private static void SetValueIfSome(PropertyInfo propertyInfo, object configInstance,
            string appsettingsKey)
        {
            var valueOption = GetSettingsValue(appsettingsKey);
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

        public static Option<object> GetSettingsValue(string settingsKey)
        {
            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Any(key => key == settingsKey))
            {
                var value = System.Configuration.ConfigurationManager.AppSettings[settingsKey];
                return Option<object>.Some(value);
            }

            Console.WriteLine($"Key {settingsKey} not available");
            return Option<object>.None();
        }
    }
}