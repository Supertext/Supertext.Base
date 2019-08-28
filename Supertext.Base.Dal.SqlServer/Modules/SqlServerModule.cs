using System.Runtime.CompilerServices;
using Autofac;
using Supertext.Base.Dal.SqlServer.ConnectionThrottling;
using Supertext.Base.Dal.SqlServer.Utils;

[assembly: InternalsVisibleTo("Supertext.Base.Dal.SqlServer.Specs")]
namespace Supertext.Base.Dal.SqlServer.Modules
{
    public class SqlServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SqlConnectionFactory>().As<ISqlConnectionFactory>();

            // Default Retry Policy provider
            builder.RegisterType<StrategyPolicyProvider>().As<IRetryPolicyProvider>().SingleInstance();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<UnitOfWorkFactory>().As<IUnitOfWorkFactory>();

            builder.RegisterType<ConnectionThrottleGuard>().As<IConnectionThrottleGuard>().SingleInstance();
        }
    }
}