using Autofac;
using Supertext.Base.Security.Cryptography;

namespace Supertext.Base.Security
{
    public class SecurityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<KeyVaultCertificateService>().As<ICertificateService>();
        }
    }
}