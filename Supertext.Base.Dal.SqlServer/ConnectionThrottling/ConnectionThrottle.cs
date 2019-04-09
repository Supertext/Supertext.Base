using System;
using System.Threading;

namespace Supertext.Base.Dal.SqlServer.ConnectionThrottling
{
    internal class ConnectionThrottle : IDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim;
        private bool _disposed;

        public ConnectionThrottle(SemaphoreSlim semaphoreSlim)
        {
            _semaphoreSlim = semaphoreSlim;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _semaphoreSlim.Release();
            }

            _disposed = true;
        }
    }
}