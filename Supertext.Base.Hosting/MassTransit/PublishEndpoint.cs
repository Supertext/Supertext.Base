using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Transactions;
using Microsoft.Extensions.Logging;
using Supertext.Base.Events;
using Supertext.Base.Messaging;

namespace Supertext.Base.Hosting.MassTransit
{
    internal class PublishEndpoint : IMessagePublisher, IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ITransactionalBus _transactionalBus;
        private readonly ILogger<PublishEndpoint> _logger;

        public PublishEndpoint(IPublishEndpoint publishEndpoint,
                               ITransactionalBus transactionalBus,
                               ILogger<PublishEndpoint> logger)
        {
            _publishEndpoint = publishEndpoint;
            _transactionalBus = transactionalBus;
            _logger = logger;
        }

        public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Sending message/event of type {typeof(TMessage).Name}.");
            return PublishAsync(message, Guid.NewGuid(), cancellationToken);
        }

        public Task PublishAsync<TMessage>(TMessage message, Guid correlationId, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Sending message/event of type {typeof(TMessage).Name} with correlation ID {correlationId}.");
            return _publishEndpoint.Publish(message, context => context.CorrelationId = correlationId, cancellationToken);
        }

        public Task PublishWithinTransactionScopeAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Sending message/event of type {typeof(TMessage).Name}.");
            return PublishWithinTransactionScopeAsync(message, Guid.NewGuid(), cancellationToken);
        }

        public Task PublishWithinTransactionScopeAsync<TMessage>(TMessage message, Guid correlationId, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Sending message/event of type {typeof(TMessage).Name} with correlation ID {correlationId}.");
            return _transactionalBus.Publish(message, context => context.CorrelationId = correlationId, cancellationToken);
        }
    }
}