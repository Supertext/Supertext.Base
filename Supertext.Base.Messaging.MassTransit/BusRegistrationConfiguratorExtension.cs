using MassTransit;

namespace Supertext.Base.Messaging.MassTransit
{
    public static class BusRegistrationConfiguratorExtension
    {
        public static void RegisterMessage<TMessage>(this IBusRegistrationConfigurator configurator)
            where TMessage : class
        {
            configurator.AddConsumer<MessageConsumer<TMessage>>();
        }
    }
}