using System;
using System.Threading.Tasks;

namespace Supertext.Base.Dal.SqlServer.ConnectionThrottling
{
    public interface IConnectionThrottleGuard : IDisposable
    {
        /// <summary>
        /// Invoked in a using block it is guarded that only a certain amount of concurrent connections
        /// to the database are allowed.
        /// Max concurrent connection is configured in a config section called 'ConnectionThrottleGuard'
        /// with the property 'MaxCountOfConcurrentSqlConnections'.
        /// The instance of IConnectionThrottleGuard is registered as single instance.
        /// </summary>
        /// <returns></returns>
        Task<IDisposable> ExecuteGuardedAsync();

        /// <summary>
        /// Invoked in a using block it is guarded that only a certain amount of concurrent connections
        /// to the database are allowed.
        /// Max concurrent connection is configured in a config section called 'ConnectionThrottleGuard'
        /// with the property 'MaxCountOfConcurrentSqlConnections'.
        /// The instance of IConnectionThrottleGuard is registered as single instance.
        /// </summary>
        /// <returns></returns>
        IDisposable ExecuteGuarded();
    }
}