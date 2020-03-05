using Autofac;
using Supertext.Base.Net.Http;
using Supertext.Base.Net.Mail;

namespace Supertext.Base.Net
{
    public class NetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpClientFactory>().As<IHttpClientFactory>();
        }
    }
}