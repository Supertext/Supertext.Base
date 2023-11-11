using System;
using System.Threading;
using System.Threading.Tasks;

namespace Supertext.Base.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken);
    Task PublishAsync<TMessage>(TMessage message, Guid correlationId, CancellationToken cancellationToken);

    Task PublishWithinTransactionScopeAsync<TMessage>(TMessage message, CancellationToken cancellationToken);
    Task PublishWithinTransactionScopeAsync<TMessage>(TMessage message, Guid correlationId, CancellationToken cancellationToken);
}