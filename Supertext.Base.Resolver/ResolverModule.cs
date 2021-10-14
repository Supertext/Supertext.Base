using Autofac;
using Supertext.Base.Resolver.Url;

namespace Supertext.Base.Resolver
{
    public class ResolverModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UrlResolver>().As<IUrlResolver>();
        }
    }
}