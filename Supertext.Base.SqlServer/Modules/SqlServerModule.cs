using System.Runtime.CompilerServices;
using Autofac;
using Supertext.Base.SqlServer.Utils;

[assembly: InternalsVisibleTo("Supertext.Base.SqlServer.Specs")]
namespace Supertext.Base.SqlServer.Modules
{
    public class SqlServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultRetryPolicyProvider>().As<IRetryPolicyProvider>().SingleInstance().PreserveExistingDefaults();
            builder.RegisterType<SqlConnectionFactory>().As<ISqlConnectionFactory>();
        }
    }
}