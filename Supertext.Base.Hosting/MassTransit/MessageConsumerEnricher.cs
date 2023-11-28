using System;

namespace Supertext.Base.Hosting.MassTransit;

internal class MessageConsumerEnricher
{
    private readonly Action<string> _messageArrivedCallback;

    public MessageConsumerEnricher(Action<string> messageArrivedCallback)
    {
        _messageArrivedCallback = messageArrivedCallback;
    }

    public void UseCorrelationId(Guid correlationId)
    {
        if (_messageArrivedCallback != null)
        {
            _messageArrivedCallback(correlationId.ToString());
        }
    }
}