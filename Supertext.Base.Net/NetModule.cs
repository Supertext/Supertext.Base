using System.Runtime.CompilerServices;
using Autofac;
using Supertext.Base.Authentication;
using Supertext.Base.Http;
using Supertext.Base.Net.Http;
using Supertext.Base.Net.Mail;

[assembly: InternalsVisibleTo("Supertext.Base.Net.Specs")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Supertext.Base.Net
{
    public class NetModule : Module
    {
        private const string SendGridMailServiceKey = "SendGrid";
        private const string FileSystemMailServiceKey = "FileSystem";
        private const string EnabledTokenCacheServiceKey = "EnabledTokenCache";
        private const string DisabledTokenCacheServiceKey = "DisabledTokenCache";

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SendGridMailService>().Keyed<IMailService>(SendGridMailServiceKey);
            builder.RegisterType<FileSystemMailService>().Keyed<IMailService>(FileSystemMailServiceKey);
#pragma warning disable CS0618
            builder.RegisterType<ProtectedHttpRequestMessageFactory>().As<IProtectedHttpRequestMessageFactory>();
#pragma warning restore CS0618
            builder.RegisterType<HttpRequestMessageBuilder>().As<IHttpRequestMessageBuilder>();
            builder.RegisterType<TokenProvider>().As<ITokenProvider>();
            builder.RegisterType<TokenCache>().Keyed<ITokenCache>(EnabledTokenCacheServiceKey).SingleInstance();
            builder.RegisterType<DisabledTokenCache>().Keyed<ITokenCache>(DisabledTokenCacheServiceKey).SingleInstance();
            builder.Register(ctx =>
                             {
                                 var config = ctx.Resolve<TokenConfig>();
                                 return ctx.ResolveKeyed<ITokenCache>(config.EnableTokenCaching ? EnabledTokenCacheServiceKey : DisabledTokenCacheServiceKey);
                             });
            builder.RegisterType<TokenEndpointProvider>().As<ITokenEndpointProvider>().SingleInstance();
            builder.RegisterType<UriBuilder>().As<IUriBuilder>().As<IHostInitializer>().InstancePerLifetimeScope();

            builder.Register(ctx =>
                             {
                                 var config = ctx.Resolve<MailServiceConfig>();
                                 return ctx.ResolveKeyed<IMailService>(config.SendGridEnabled ? SendGridMailServiceKey : FileSystemMailServiceKey);
                             });
        }
    }
}