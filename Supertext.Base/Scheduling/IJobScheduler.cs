using System;

namespace Supertext.Base.Scheduling
{
    public interface IJobScheduler<TJobPayload>
    {
        void ScheduleJob(Job<TJobPayload> job);

        void CancelJob(Guid jobId);

        bool IsQueueEmpty();
    }
}