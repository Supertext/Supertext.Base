using System;
using System.Data;

namespace Supertext.Base.Dal
{
    public interface IUnitOfWork
    {
        void ExecuteWithinTransactionScope(Action<IDbConnection> action);
    }
}