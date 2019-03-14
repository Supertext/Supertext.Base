using System;
using System.Data;
using System.Transactions;
using Supertext.Base.Dal.SqlServer.Utils;

namespace Supertext.Base.Dal
{
    public interface IUnitOfWork
    {
        void ExecuteWithinTransactionScope(Action<IDbConnection> action);
    }

    internal class UnitOfWork : IUnitOfWork
    {
        private readonly string _connectionString;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public UnitOfWork(string connectionString, ISqlConnectionFactory sqlConnectionFactory)
        {
            _connectionString = connectionString;
            _sqlConnectionFactory = sqlConnectionFactory;
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
    }
}