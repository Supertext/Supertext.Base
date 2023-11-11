using Autofac;
using MassTransit;
using MassTransit.Transactions;

namespace Supertext.Base.Messaging.MassTransit
{
    public class MessagingMassTransitModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PublishEndpoint>().As<IMessagePublisher>();
            builder.Register(context =>
                             {
                                 var bus = context.Resolve<IBusControl>();
                                 return new TransactionalEnlistmentBus(bus);
                             })
                   .As<ITransactionalBus>().InstancePerLifetimeScope();
        }
    }
}