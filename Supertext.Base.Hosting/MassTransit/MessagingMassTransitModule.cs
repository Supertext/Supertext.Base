using Autofac;
using MassTransit;
using MassTransit.Transactions;
using Supertext.Base.Events;
using Supertext.Base.Messaging;

namespace Supertext.Base.Hosting.MassTransit
{
    public class MessagingMassTransitModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PublishEndpoint>().As<IMessagePublisher>().As<IEventPublisher>();
            builder.Register(context =>
                             {
                                 var bus = context.Resolve<IBusControl>();
                                 return new TransactionalEnlistmentBus(bus);
                             })
                   .As<ITransactionalBus>().InstancePerLifetimeScope();
        }
    }
}