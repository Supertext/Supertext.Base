using Autofac;
using Supertext.Base.Identity.UserInfo;

namespace Supertext.Base.Identity
{
    public class IdentityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserInfoProvider>().As<IUserInfoProvider>();
        }
    }
}