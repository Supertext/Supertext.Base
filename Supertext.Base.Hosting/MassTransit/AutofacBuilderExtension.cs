using System;
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
    /// <param name="messageCorrelationIdProvider">Provides the correlation of the arrived message.</param>
    /// <returns></returns>
    public static ContainerBuilder RegisterMessageConsumers(this ContainerBuilder builder,
                                                            Assembly assembly,
                                                            Action<string> messageCorrelationIdProvider = null)
    {
        var enricher = new MessageConsumerEnricher(messageCorrelationIdProvider);
        builder.RegisterInstance(enricher).AsSelf().SingleInstance();
        builder.RegisterModule<MessagingMassTransitModule>();
        builder.RegisterAssemblyTypes(assembly)
               .AsClosedTypesOf(typeof(IMessageConsumer<>))
               .AsImplementedInterfaces();
        return builder;
    }
}