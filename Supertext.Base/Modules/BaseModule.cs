using System.Runtime.CompilerServices;
using Autofac;
using Supertext.Base.Common;
using Supertext.Base.Factory;
[assembly: InternalsVisibleTo("Supertext.Base.Tests")]
[assembly: InternalsVisibleTo("Supertext.Base.Hosting")]
namespace Supertext.Base.Modules
{
    public class BaseModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>();
            builder.RegisterType<WildcardChecker>().As<IWildcardChecker>();

            RegisterFactories(builder);
        }

        private void RegisterFactories(ContainerBuilder builder)
        {
            builder.RegisterType<AutofacFactory>().As<IFactory>();
            builder.RegisterGeneric(typeof(AutofacFactory<>)).As(typeof(IFactory<>));
            builder.RegisterGeneric(typeof(AutofacFactory<,>)).As(typeof(IFactory<,>));
            builder.RegisterGeneric(typeof(AutofacFactory<,,>)).As(typeof(IFactory<,,>));
            builder.RegisterGeneric(typeof(AutofacFactory<,,,>)).As(typeof(IFactory<,,,>));

            builder.RegisterGeneric(typeof(AutofacKeyFactory<,>)).As(typeof(IKeyFactory<,>));

            builder.RegisterType<SequentialGuidFactory>().As<ISequentialGuidFactory>();
        }
    }
}