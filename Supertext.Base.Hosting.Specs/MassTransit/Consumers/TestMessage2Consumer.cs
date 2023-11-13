using System;
using System.Threading;
using System.Threading.Tasks;
using Supertext.Base.Common;
using Supertext.Base.Hosting.Specs.MassTransit.Messages;
using Supertext.Base.Messaging;

namespace Supertext.Base.Hosting.Specs.MassTransit.Consumers;

public class TestMessage2Consumer : IMessageConsumer<TestMessage2>
{
    private readonly ConsumerHelper _consumerHelper;

    public TestMessage2Consumer(ConsumerHelper consumerHelper)
    {
        _consumerHelper = consumerHelper;
    }

    public Task HandleAsync(TestMessage2 message, Option<Guid> contextCorrelationId, CancellationToken contextCancellationToken)
    {
        _consumerHelper.CorrelationId = contextCorrelationId;

        return Task.CompletedTask;
    }
}