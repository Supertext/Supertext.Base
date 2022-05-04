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
#pragma warning disable CS0618
            builder.RegisterType<ProtectedHttpRequestMessageFactory>().As<IProtectedHttpRequestMessageFactory>();
#pragma warning restore CS0618
            builder.RegisterType<HttpRequestMessageBuilder>().As<IHttpRequestMessageBuilder>();
            builder.RegisterType<TokenProvider>().As<ITokenProvider>();
            builder.RegisterType<UriBuilder>().As<IUriBuilder>().As<IHostInitializer>().InstancePerLifetimeScope();
        }
    }
}