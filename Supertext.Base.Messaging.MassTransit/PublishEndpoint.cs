using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Supertext.Base.Messaging.MassTransit
{
    internal class PublishEndpoint : IMessagePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<PublishEndpoint> _logger;

        public PublishEndpoint(IPublishEndpoint publishEndpoint, ILogger<PublishEndpoint> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : class, new()
        {
            _logger.LogDebug($"Sending message of type {typeof(TMessage).Name}.");
            return _publishEndpoint.Publish(message, cancellationToken);
        }

        public Task SendAsync<TMessage>(TMessage message, Guid correlationId, CancellationToken cancellationToken) where TMessage : class, new()
        {
            _logger.LogDebug($"Sending message of type {typeof(TMessage).Name} with correlation ID {correlationId}.");
            return _publishEndpoint.Publish(message, context => context.CorrelationId = correlationId, cancellationToken);
        }
    }
}