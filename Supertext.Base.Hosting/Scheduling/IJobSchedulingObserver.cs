using System.Threading;
using System.Threading.Tasks;
using Supertext.Base.Scheduling;

namespace Supertext.Base.Hosting.Scheduling
{
    internal interface IJobSchedulingObserver<TJobPayload>
    {
        Task<Job<TJobPayload>> DequeueAsync(CancellationToken cancellationToken);
    }
}