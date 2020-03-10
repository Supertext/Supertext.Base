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
            builder.RegisterType<HttpClientFactory>().As<IHttpClientFactory>();
            builder.RegisterType<MailService>().As<IMailService>();
        }
    }
}