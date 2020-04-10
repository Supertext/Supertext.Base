using Autofac;

namespace Supertext.Base.Test.Mvc
{
    internal class MvcModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TestSettings>().AsSelf().SingleInstance();
        }
    }
}