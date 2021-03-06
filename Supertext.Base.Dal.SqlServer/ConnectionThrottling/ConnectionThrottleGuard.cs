﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Supertext.Base.Dal.SqlServer.ConnectionThrottling
{
    internal class ConnectionThrottleGuard : IConnectionThrottleGuard
    {
        private readonly SemaphoreSlim _semaphore;
        private bool _disposed;

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
                _semaphore?.Dispose();
            }

            _disposed = true;
        }
    }
}