using Autofac;

namespace Supertext.Base.Messaging.MassTransit
{
    public class MessagingMassTransitModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PublishEndpoint>().As<IMessagePublisher>();
        }
    }
}