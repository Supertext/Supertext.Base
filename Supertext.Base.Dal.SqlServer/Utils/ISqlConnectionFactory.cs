using System.Data;

namespace Supertext.Base.Dal.SqlServer.Utils
{
    public interface ISqlConnectionFactory
    {
        IDbConnection CreateOpenedReliableConnection(string connectionString);
    }
}