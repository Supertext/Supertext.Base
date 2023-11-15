using MassTransit;

namespace Supertext.Base.Hosting.MassTransit
{
    public static class BusRegistrationConfiguratorExtension
    {
        public static void RegisterMessage<TMessage>(this IBusRegistrationConfigurator configurator, int? concurrentMessageLimit = null)
            where TMessage : class
        {
            configurator.AddConsumer<MessageConsumer<TMessage>>(consumerConfigurator =>
                                                                    consumerConfigurator.ConcurrentMessageLimit = concurrentMessageLimit);
        }
    }
}