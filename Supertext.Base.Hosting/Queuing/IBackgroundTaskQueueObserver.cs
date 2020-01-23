using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Supertext.Base.Abstractions;

namespace Supertext.Base.Hosting.Queuing
{
    public interface IBackgroundTaskQueueObserver
    {
        Task<Func<ILifetimeScopeAbstraction, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);

        void WorkItemFinished();
    }
}