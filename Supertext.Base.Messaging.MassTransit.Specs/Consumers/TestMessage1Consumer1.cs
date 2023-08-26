using Supertext.Base.Common;
using Supertext.Base.Messaging.MassTransit.Specs.Messages;

namespace Supertext.Base.Messaging.MassTransit.Specs.Consumers;

public class TestMessage1Consumer1 : IMessageConsumer<TestMessage1>
{
    private readonly ConsumerHelper _consumerHelper;

    public TestMessage1Consumer1(ConsumerHelper consumerHelper)
    {
        _consumerHelper = consumerHelper;
    }

    public Task HandleAsync(TestMessage1 message, Option<Guid> contextCorrelationId, CancellationToken contextCancellationToken)
    {
        _consumerHelper.TestConsumer1WasCalled(message, contextCorrelationId);
        return Task.CompletedTask;
    }
}