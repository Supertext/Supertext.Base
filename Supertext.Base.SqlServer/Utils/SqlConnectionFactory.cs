using System.Data;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace Supertext.Base.SqlServer.Utils
{
    internal class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly IRetryPolicyProvider _retryPolicyProvider;

        public SqlConnectionFactory(IRetryPolicyProvider retryPolicyProvider)
        {
            _retryPolicyProvider = retryPolicyProvider;
        }

        public IDbConnection CreateOpenedReliableConnection(string connectionString)
        {
            var conn = new ReliableSqlConnection(connectionString, _retryPolicyProvider.RetryPolicy);

            conn.Open(_retryPolicyProvider.RetryPolicy);

            return conn;
        }
    }
}