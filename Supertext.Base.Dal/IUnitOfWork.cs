using System;
using System.Data;
using System.Threading.Tasks;

namespace Supertext.Base.Dal
{
    public interface IUnitOfWork
    {
        TReturnValue ExecuteScalar<TReturnValue>(Func<IDbConnection, TReturnValue> action);

        Task<TReturnValue> ExecuteScalarAsync<TReturnValue>(Func<IDbConnection, TReturnValue> action);

        void ExecuteWithinTransactionScope(Action<IDbConnection> action);

        Task ExecuteWithinTransactionScopeAsync(Action<IDbConnection> action);
    }
}