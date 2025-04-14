using System.Runtime.CompilerServices;
using Autofac;
using Supertext.Base.Authentication;
using Supertext.Base.Http;
using Supertext.Base.Net.Http;
using Supertext.Base.Net.Mail;

[assembly:InternalsVisibleTo("Supertext.Base.Net.Specs")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Supertext.Base.Net
{
    public class NetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SendGridMailService>().Keyed<IMailService>(true);
            builder.RegisterType<FileSystemMailService>().Keyed<IMailService>(false);
#pragma warning disable CS0618
            builder.RegisterType<ProtectedHttpRequestMessageFactory>().As<IProtectedHttpRequestMessageFactory>();
#pragma warning restore CS0618
            builder.RegisterType<HttpRequestMessageBuilder>().As<IHttpRequestMessageBuilder>();
            builder.RegisterType<TokenProvider>().As<ITokenProvider>();
            builder.RegisterType<UriBuilder>().As<IUriBuilder>().As<IHostInitializer>().InstancePerLifetimeScope();

            builder.Register(ctx => {
                                        var config = ctx.Resolve<MailServiceConfig>();
                                        var keyedService = ctx.ResolveKeyed<IMailService>(config.SendGridEnabled);
                                        return keyedService;
                                    });
        }
    }
}