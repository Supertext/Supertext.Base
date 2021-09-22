using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Supertext.Base.Common;
using Supertext.Base.Scheduling;

namespace Supertext.Base.Hosting.Scheduling
{
    internal class JobSchedulingQueue<TJobPayload> : IJobSchedulingObserver<TJobPayload>, IJobScheduler<TJobPayload>, IDisposable
    {
        private readonly ConcurrentQueue<Job<TJobPayload>> _workItems = new ConcurrentQueue<Job<TJobPayload>>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void ScheduleJob(Job<TJobPayload> job)
        {
            Validate.NotNull(job);
            _workItems.Enqueue(job);
            _signal.Release();
        }

        public void CancelJob(Guid jobId)
        {
            ScheduleJob(new JobCancellation<TJobPayload>(jobId));
        }

        public async Task<Job<TJobPayload>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken).ConfigureAwait(false);
            _workItems.TryDequeue(out var workItem);
            return workItem;
        }

        public bool IsQueueEmpty()
        {
            return _workItems.IsEmpty;
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

    internal class JobCancellation<TJobPayload> : Job<TJobPayload>
    {
        public JobCancellation(Guid jobId) : base((jobId))
        {
        }
    }
}