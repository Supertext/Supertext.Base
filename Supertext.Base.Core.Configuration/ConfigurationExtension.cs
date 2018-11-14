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
                .OnActivated(setting => SettingActivated(setting, configuration));
        }

        private static void SettingActivated(IActivatedEventArgs<object> obj, IConfiguration configuration)
        {
            var section = obj.Instance.GetType().GetCustomAttribute<ConfigSectionAttribute>();
            if (section != null)
            {
                configuration.GetSection(section.SectionName).Bind(obj.Instance);
            }
        }
    }
}