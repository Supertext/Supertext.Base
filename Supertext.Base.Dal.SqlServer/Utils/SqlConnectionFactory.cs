using System.Data;
using System.Data.SqlClient;

namespace Supertext.Base.Dal.SqlServer.Utils
{
    internal class SqlConnectionFactory : ISqlConnectionFactory
    {
        public IDbConnection CreateOpenedReliableConnection(string connectionString)
        {
            var conn = new SqlConnection(connectionString);

            conn.Open();

            return conn;
        }
    }
}