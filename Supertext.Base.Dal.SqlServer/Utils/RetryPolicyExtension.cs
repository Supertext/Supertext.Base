using Autofac;

namespace Supertext.Base.Dal.SqlServer.Utils
{
    public static class RetryPolicyExtension
    {
        public static void UseStratPolRetryPolicy(this ContainerBuilder builder)
        {
            builder.RegisterType<StratPolPolicyProvider>().As<IRetryPolicyProvider>().SingleInstance();
        }
    }
}