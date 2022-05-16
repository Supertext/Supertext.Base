using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Supertext.Base.BackgroundTasks;
using Supertext.Base.Common;
using Supertext.Base.Factory;

namespace Supertext.Base.Hosting.Queuing
{
    internal class BackgroundTaskQueue : IBackgroundTaskQueueObserver, IBackgroundTaskQueue, IDisposable
    {
        private readonly ConcurrentQueue<WorkItem> _workItems = new();
        private readonly SemaphoreSlim _signal = new(0);
        private volatile bool _taskPending;

        public void QueueBackgroundWorkItem(Func<IFactory, CancellationToken, Task> workItem, Guid correlationId = default)
        {
            Validate.NotNull(workItem);
            _workItems.Enqueue(new WorkItem(workItem, correlationId == Guid.Empty ? Guid.NewGuid() : correlationId));
            _signal.Release();
        }

        public async Task<WorkItem> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken).ConfigureAwait(false);
            _workItems.TryDequeue(out var workItem);
            _taskPending = true;
            return workItem;
        }

        public void WorkItemFinished()
        {
            _taskPending = false;
        }

        public bool IsQueueEmpty()
        {
            return _workItems.IsEmpty && !_taskPending;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _signal?.Dispose();
            }
        }
    }
}