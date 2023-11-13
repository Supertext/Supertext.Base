using System;

namespace Supertext.Base.Hosting.Specs.MassTransit.Messages;

public class TestMessage2
{
    public int Id { get; init; }

    public Guid CorrelationId { get; set;}
}