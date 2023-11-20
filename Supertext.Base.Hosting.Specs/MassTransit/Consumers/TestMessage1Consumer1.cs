using System;
using System.Threading;
using System.Threading.Tasks;
using Supertext.Base.Common;
using Supertext.Base.Hosting.Specs.MassTransit.Messages;
using Supertext.Base.Messaging;

namespace Supertext.Base.Hosting.Specs.MassTransit.Consumers;

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