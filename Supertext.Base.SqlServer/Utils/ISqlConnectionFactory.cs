using System.Data;

namespace Supertext.Base.SqlServer.Utils
{
    public interface ISqlConnectionFactory
    {
        IDbConnection CreateOpenedReliableConnection(string connectionString);
    }
}