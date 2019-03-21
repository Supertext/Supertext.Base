using System;
using System.Data;
using System.Threading.Tasks;

namespace Supertext.Base.Dal
{
    public interface IUnitOfWork
    {
        TReturnValue ExecuteScalar<TReturnValue>(Func<IDbConnection, TReturnValue> func);

        Task<TReturnValue> ExecuteScalarAsync<TReturnValue>(Func<IDbConnection, Task<TReturnValue>>  func);

        void ExecuteWithinTransactionScope(Action<IDbConnection> action);

        Task ExecuteWithinTransactionScopeAsync(Func<IDbConnection, Task> func);
    }
}