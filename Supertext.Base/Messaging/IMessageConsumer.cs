using System;
using System.Threading;
using System.Threading.Tasks;
using Supertext.Base.Common;

namespace Supertext.Base.Messaging;

public interface IMessageConsumer<in TMessage>
{
    Task HandleAsync(TMessage message, Option<Guid> contextCorrelationId, CancellationToken contextCancellationToken);
}