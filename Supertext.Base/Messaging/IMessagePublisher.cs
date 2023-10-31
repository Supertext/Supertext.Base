using System;
using System.Threading;
using System.Threading.Tasks;

namespace Supertext.Base.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : class, new();
    Task PublishAsync<TMessage>(TMessage message, Guid correlationId, CancellationToken cancellationToken) where TMessage : class, new();
}