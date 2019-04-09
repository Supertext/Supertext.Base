using System;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using Supertext.Base.Dal.SqlServer.ConnectionThrottling;
using Supertext.Base.Dal.SqlServer.Utils;

namespace Supertext.Base.Dal.SqlServer
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly string _connectionString;
        private readonly IConnectionThrottleGuard _connectionThrottleGuard;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public UnitOfWork(string connectionString,
                          ISqlConnectionFactory sqlConnectionFactory,
                          IConnectionThrottleGuard connectionThrottleGuard)
        {
            _connectionString = connectionString;
            _sqlConnectionFactory = sqlConnectionFactory;
            _connectionThrottleGuard = connectionThrottleGuard;
        }

        public TReturnValue ExecuteScalar<TReturnValue>(Func<IDbConnection, TReturnValue> action)
        {
            using (_connectionThrottleGuard.ExecuteGuarded())
            using (var connection = _sqlConnectionFactory.CreateOpenedReliableConnection(_connectionString))
            {
                return action(connection);
            }
        }

        public async Task<TReturnValue> ExecuteScalarAsync<TReturnValue>(Func<IDbConnection, Task<TReturnValue>> action)
        {
            using (await _connectionThrottleGuard.ExecuteGuardedAsync())
            using (var connection = _sqlConnectionFactory.CreateOpenedReliableConnection(_connectionString))
            {
                return await action(connection).ConfigureAwait(false);
            }
        }

        public void ExecuteWithinTransactionScope(Action<IDbConnection> action)
        {
            using (_connectionThrottleGuard.ExecuteGuarded())
            {
                ExecuteWithinTransactionScopeAsync(connection =>
                                                   {
                                                       action(connection);
                                                       return Task.CompletedTask;
                                                   })
                    .GetAwaiter()
                    .GetResult();
            }
        }

        public TReturnValue ExecuteWithinTransactionScope<TReturnValue>(Func<IDbConnection, TReturnValue> func)
        {
            using (_connectionThrottleGuard.ExecuteGuarded())
            {
                return ExecuteWithinTransactionScopeAsync(connection =>
                                                          {
                                                              var result = func(connection);
                                                              return Task.FromResult(result);
                                                          })
                       .GetAwaiter()
                       .GetResult();
            }
        }

        public async Task ExecuteWithinTransactionScopeAsync(Func<IDbConnection, Task> func)
        {
            using (await _connectionThrottleGuard.ExecuteGuardedAsync())
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var connection = _sqlConnectionFactory.CreateOpenedReliableConnection(_connectionString))
            {
                await func(connection).ConfigureAwait(false);
                scope.Complete();
            }
        }

        public async Task<TReturnValue> ExecuteWithinTransactionScopeAsync<TReturnValue>(Func<IDbConnection, Task<TReturnValue>> func)
        {
            using (await _connectionThrottleGuard.ExecuteGuardedAsync())
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var connection = _sqlConnectionFactory.CreateOpenedReliableConnection(_connectionString))
            {
                var result = await func(connection).ConfigureAwait(false);
                scope.Complete();
                return result;
            }
        }

        public void Dispose()
        {
            _connectionThrottleGuard?.Dispose();
        }
    }
}