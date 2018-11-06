using Autofac;
using Supertext.Base.Configuration;

namespace Supertext.Base.NetFramework.Configuration
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigurationManager>().As<IConfigurationManager>();
        }
    }
}