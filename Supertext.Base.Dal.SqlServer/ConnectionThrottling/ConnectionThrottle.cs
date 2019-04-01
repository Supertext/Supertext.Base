using System;
using System.Threading;

namespace Supertext.Base.Dal.SqlServer.ConnectionThrottling
{
    internal class ConnectionThrottle : IDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        public ConnectionThrottle(SemaphoreSlim semaphoreSlim)
        {
            _semaphoreSlim = semaphoreSlim;
        }

        public void Dispose()
        {
            _semaphoreSlim.Release();
        }
    }
}