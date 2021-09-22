using System;
using System.Threading;
using System.Threading.Tasks;
using Supertext.Base.Common;
using Supertext.Base.Factory;

namespace Supertext.Base.Scheduling
{
    public class Job<TPayload>
    {
        protected Job(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// Job
        /// </summary>
        /// <param name="id">Must be unique. Jobs can be cancelled with that id.</param>
        /// <param name="dueTime"></param>
        /// <param name="payload"></param>
        /// <param name="workItem"></param>
        public Job(Guid id, TimeSpan dueTime, TPayload payload,
                   Func<IFactory, TPayload, CancellationToken, Task> workItem)
        {
            Validate.NotNull(id);
            Validate.NotNull(dueTime);
            Validate.NotNull(payload);
            Validate.NotNull(workItem);

            Id = id;
            DueTime = dueTime;
            Payload = payload;
            WorkItem = workItem;
        }

        /// <summary>
        /// Unique Id to schedule a job.
        /// </summary>
        public Guid Id { get; }

        public TimeSpan DueTime { get; }

        public TPayload Payload { get; }

        public Func<IFactory, TPayload, CancellationToken, Task> WorkItem { get; }
    }
}