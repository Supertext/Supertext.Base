using System.Runtime.CompilerServices;
using Autofac;
using Supertext.Base.Security.Cryptography;
using Supertext.Base.Security.NWebSec;

[assembly: InternalsVisibleTo("Supertext.Base.Security.Specs")]
namespace Supertext.Base.Security
{
    public class SecurityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<KeyVaultCertificateService>().As<ICertificateService>();
            builder.RegisterType<HeaderConfigurationParser>().As<IHeaderConfigurationParser>();
        }
    }
}