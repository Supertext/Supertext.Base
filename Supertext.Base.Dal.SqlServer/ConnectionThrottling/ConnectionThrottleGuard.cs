using System;
using System.Threading;
using System.Threading.Tasks;

namespace Supertext.Base.Dal.SqlServer.ConnectionThrottling
{
    internal class ConnectionThrottleGuard : IConnectionThrottleGuard
    {
        private readonly SemaphoreSlim _semaphore;

        public ConnectionThrottleGuard(ThrottlingConfig throttlingConfig)
        {
            _semaphore = new SemaphoreSlim(throttlingConfig.MaxCountOfConcurrentSqlConnections);
        }

        public IDisposable ExecuteGuarded()
        {
            _semaphore.Wait();
            return new ConnectionThrottle(_semaphore);
        }

        public async Task<IDisposable> ExecuteGuardedAsync()
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            return new ConnectionThrottle(_semaphore);
        }

        public void Dispose()
        {
            _semaphore?.Dispose();
        }
    }
}