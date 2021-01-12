using System.Threading;
using System.Threading.Tasks;
using Supertext.Base.Factory;

namespace Supertext.Base.Scheduling
{
    public interface IScheduledsJobResumer
    {
        Task ResumeAsync(IFactory factory, CancellationToken cancellationToken);
    }
}