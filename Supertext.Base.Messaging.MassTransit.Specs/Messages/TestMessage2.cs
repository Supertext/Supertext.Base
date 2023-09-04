namespace Supertext.Base.Messaging.MassTransit.Specs.Messages;

public class TestMessage2
{
    public int Id { get; init; }

    public Guid CorrelationId { get; set;}
}