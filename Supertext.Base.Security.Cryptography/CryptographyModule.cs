using Autofac;
using Supertext.Base.Security.Cryptography.Encryption;

namespace Supertext.Base.Security.Cryptography
{
    public class CryptographyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Sha256Encryptor>().As<ISha256Encryptor>();
        }
    }
}