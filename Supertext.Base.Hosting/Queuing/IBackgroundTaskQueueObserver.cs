using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;

namespace Supertext.Base.Hosting.Queuing
{
    public interface IBackgroundTaskQueueObserver
    {
        Task<Func<ILifetimeScope, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);

        void WorkItemFinished();
    }
}