using System;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using Supertext.Base.Dal.SqlServer.Utils;

namespace Supertext.Base.Dal.SqlServer
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly string _connectionString;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public UnitOfWork(string connectionString, ISqlConnectionFactory sqlConnectionFactory)
        {
            _connectionString = connectionString;
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public TReturnValue ExecuteScalar<TReturnValue>(Func<IDbConnection, TReturnValue> func)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var connection = _sqlConnectionFactory.CreateOpenedReliableConnection(_connectionString))
            {
                var result = func(connection);
                scope.Complete();
                return result;
            }
        }

        public async Task<TReturnValue> ExecuteScalarAsync<TReturnValue>(Func<IDbConnection, Task<TReturnValue>> action)
        {
            return await ExecuteScalar(action);
        }

        public void ExecuteWithinTransactionScope(Action<IDbConnection> action)
        {
            ExecuteWithinTransactionScopeAsync(connection =>
                                               {
                                                   action(connection);
                                                   return Task.CompletedTask;
                                               })
                .GetAwaiter()
                .GetResult();
        }

        public async Task ExecuteWithinTransactionScopeAsync(Func<IDbConnection, Task> func)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var connection = _sqlConnectionFactory.CreateOpenedReliableConnection(_connectionString))
            {
                await func(connection);
                scope.Complete();
            }
        }
    }
}