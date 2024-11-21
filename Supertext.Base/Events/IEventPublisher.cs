using System;
using System.Threading;
using System.Threading.Tasks;

namespace Supertext.Base.Events;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent payload, CancellationToken ct);
    Task PublishAsync<TEvent>(TEvent payload, Guid correlationId, CancellationToken ct);

    Task PublishWithinTransactionScopeAsync<TEvent>(TEvent payload, CancellationToken ct);
    Task PublishWithinTransactionScopeAsync<TEvent>(TEvent payload, Guid correlationId, CancellationToken ct);
}