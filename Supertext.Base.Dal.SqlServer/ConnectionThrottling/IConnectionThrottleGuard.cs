using System;
using System.Threading.Tasks;

namespace Supertext.Base.Dal.SqlServer.ConnectionThrottling
{
    public interface IConnectionThrottleGuard : IDisposable
    {
        Task<IDisposable> ExecuteGuardedAsync();
        IDisposable ExecuteGuarded();
    }
}