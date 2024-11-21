using System.Reflection;
using Autofac;
using Supertext.Base.Messaging;

namespace Supertext.Base.Hosting.MassTransit;

public static class AutofacBuilderExtension
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static ContainerBuilder RegisterMessageConsumers(this ContainerBuilder builder, Assembly assembly)
    {
        builder.RegisterModule<MessagingMassTransitModule>();
        builder.RegisterAssemblyTypes(assembly)
               .AsClosedTypesOf(typeof(IMessageConsumer<>))
               .AsImplementedInterfaces();
        return builder;
    }
}