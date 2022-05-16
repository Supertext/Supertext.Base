using System;
using System.Threading;
using System.Threading.Tasks;
using Supertext.Base.Factory;

namespace Supertext.Base.Hosting.Queuing;

public struct WorkItem
{
    public Guid CorrelationId { get; }
    public Func<IFactory, CancellationToken, Task> Func { get; }

    public WorkItem(Func<IFactory, CancellationToken, Task> func, Guid correlationId)
    {
        CorrelationId = correlationId;
        Func = func;
    }
}