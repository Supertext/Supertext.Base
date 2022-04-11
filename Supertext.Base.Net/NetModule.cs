using System.Runtime.CompilerServices;
using Autofac;
using Supertext.Base.Authentication;
using Supertext.Base.Http;
using Supertext.Base.Net.Http;
using Supertext.Base.Net.Mail;

[assembly:InternalsVisibleTo("Supertext.Base.Net.Specs")]
namespace Supertext.Base.Net
{
    public class NetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MailService>().As<IMailService>();
            builder.RegisterType<ProtectedHttpRequestMessageFactory>().As<IProtectedHttpRequestMessageFactory>();
            builder.RegisterType<TokenProvider>().As<ITokenProvider>();
            builder.RegisterType<UriBuilder>().As<IUriBuilder>().As<IDomainInitializer>().As<IHostInitializer>().InstancePerLifetimeScope();
        }
    }
}