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

        public TReturnValue ExecuteScalar<TReturnValue>(Func<IDbConnection, TReturnValue> action)
        {
            using (var connection = _sqlConnectionFactory.CreateOpenedReliableConnection(_connectionString))
            {
                return action(connection);
            }
        }

        public async Task<TReturnValue> ExecuteScalarAsync<TReturnValue>(Func<IDbConnection, TReturnValue> action)
        {
            var result = ExecuteScalar(action);
            return await Task.FromResult(result);
        }

        public void ExecuteWithinTransactionScope(Action<IDbConnection> action)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var connection = _sqlConnectionFactory.CreateOpenedReliableConnection(_connectionString))
            {
                action(connection);
                scope.Complete();
            }
        }

        public Task ExecuteWithinTransactionScopeAsync(Action<IDbConnection> action)
        {
            ExecuteWithinTransactionScope(action);
            return Task.CompletedTask;
        }
    }
}