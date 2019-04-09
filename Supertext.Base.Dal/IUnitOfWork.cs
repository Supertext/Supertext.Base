using System;
using System.Data;
using System.Threading.Tasks;

namespace Supertext.Base.Dal
{
    public interface IUnitOfWork : IDisposable
    {
        TReturnValue ExecuteScalar<TReturnValue>(Func<IDbConnection, TReturnValue> func);

        Task<TReturnValue> ExecuteScalarAsync<TReturnValue>(Func<IDbConnection, Task<TReturnValue>>  func);

        void ExecuteWithinTransactionScope(Action<IDbConnection> action);

        TReturnValue ExecuteWithinTransactionScope<TReturnValue>(Func<IDbConnection, TReturnValue> func);

        Task ExecuteWithinTransactionScopeAsync(Func<IDbConnection, Task> func);

        Task<TReturnValue> ExecuteWithinTransactionScopeAsync<TReturnValue>(Func<IDbConnection, Task<TReturnValue>> func);
    }
}