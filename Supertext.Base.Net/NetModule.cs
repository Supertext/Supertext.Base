using System.Runtime.CompilerServices;
using Autofac;
using Supertext.Base.Net.Http;
using Supertext.Base.Net.Mail;

[assembly:InternalsVisibleTo("Supertext.Base.Net.Specs")]
namespace Supertext.Base.Net
{
    public class NetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
#pragma warning disable 618 // => must be removed, when Supertext.Base.Http.IHttpClientFactory is removed
            builder.RegisterType<HttpClientFactory>().As<IHttpClientFactory>();
#pragma warning restore 618
            builder.RegisterType<MailService>().As<IMailService>();
        }
    }
}