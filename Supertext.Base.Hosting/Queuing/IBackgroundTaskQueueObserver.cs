using System;
using System.Threading;
using System.Threading.Tasks;
using Supertext.Base.Factory;

namespace Supertext.Base.Hosting.Queuing
{
    public interface IBackgroundTaskQueueObserver
    {
        Task<Func<IFactory, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);

        void WorkItemFinished();
    }
}