using System.Threading;
using System.Threading.Tasks;

namespace Supertext.Base.Hosting.Queuing
{
    public interface IBackgroundTaskQueueObserver
    {
        Task<WorkItem> DequeueAsync(CancellationToken cancellationToken);

        void WorkItemFinished();
    }
}