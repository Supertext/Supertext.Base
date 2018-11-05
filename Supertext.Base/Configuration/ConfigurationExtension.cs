using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Supertext.Base.Common;

namespace Supertext.Base.Configuration
{
    public static class ConfigurationExtension
    {
        public static void RegisterConfigurationsWithAppConfigValues(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            Validate.NotNull(assemblies, nameof(assemblies));
            
            foreach (var assembly in assemblies)
            {
                var configTypes = assembly.GetTypes().Where(type => type.GetTypeInfo().IsAssignableTo<IConfiguration>()).ToList();
                RegisterAndInitializeConfigTypes(builder, configTypes);
            }
        }

        private static void RegisterAndInitializeConfigTypes(ContainerBuilder builder, ICollection<Type> configTypes)
        {
            foreach (var configType in configTypes)
            {
                var config = Activator.CreateInstance(configType);

                builder.RegisterInstance(config).As(configType).OnActivated(SetupValues);
            }
        }

        private static void SetupValues(IActivatedEventArgs<object> args)
        {
            var configurationManager = args.Context.Resolve<IConfigurationManager>();
            var configInstance = args.Instance;
            var properties = configInstance.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                var settingKeys = propertyInfo.GetCustomAttributes<SettingsKeyAttribute>().ToList();
                if (settingKeys.Any())
                {
                    settingKeys.ForEach(key => SetValueIfSome(propertyInfo, configInstance, configurationManager, key));
                    continue;
                }

                var valueOption = configurationManager.GetSettingsValue(propertyInfo.Name);
                if (valueOption.IsSome)
                {
                    propertyInfo.SetValue(configInstance, Convert(valueOption.Value, propertyInfo.PropertyType));
                }
            }
        }

        private static void SetValueIfSome(PropertyInfo propertyInfo, object configInstance, IConfigurationManager configurationManager,
            SettingsKeyAttribute key)
        {
            var valueOption = configurationManager.GetSettingsValue(key.AppSettingsKey);
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
    }
}