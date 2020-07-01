using Autofac;
using Supertext.Base.Security.KeyVault.Keys;

namespace Supertext.Base.Security.KeyVault
{
    public class SecurityKeyVaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<KeyVaultKeysProvider>().As<IKeyVaultKeysProvider>();
        }
    }
}