using Supertext.Base.Common;
using Supertext.Base.Messaging.MassTransit.Specs.Messages;

namespace Supertext.Base.Messaging.MassTransit.Specs.Consumers;

public class ConsumerHelper
{
    public TestMessage1? Consumer1TestMessage1 { get; set; }

    public TestMessage1? Consumer2TestMessage1 { get; set; }

    public Option<Guid> CorrelationId { get; set; }

    public void TestConsumer1WasCalled(TestMessage1 message, Option<Guid> contextCorrelationId)
    {
        Consumer1TestMessage1 = message;
    }

    public void TestConsumer2WasCalled(TestMessage1 message, Option<Guid> contextCorrelationId)
    {
        Consumer2TestMessage1 = message;
    }
}