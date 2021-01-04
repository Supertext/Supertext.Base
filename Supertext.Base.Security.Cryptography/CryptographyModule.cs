using System.Runtime.CompilerServices;
using Autofac;
using Supertext.Base.Security.Cryptography.AesEncryption;
using Supertext.Base.Security.Cryptography.Hashing;
using Supertext.Base.Security.Cryptography.Hashing.Salt;
using Supertext.Base.Security.Hashing;

[assembly: InternalsVisibleTo("Supertext.Base.Security.Cryptography.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Supertext.Base.Security.Cryptography
{
    public class CryptographyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SaltGenerator>().As<ISaltGenerator>();
            builder.RegisterType<Sha256Hasher>().As<ISha256Hasher>();
            builder.RegisterType<Sha256HashValidator>().As<ISha256HashValidator>();
            builder.RegisterType<AesEncryptor>().As<IAesEncryptor>();
        }
    }
}