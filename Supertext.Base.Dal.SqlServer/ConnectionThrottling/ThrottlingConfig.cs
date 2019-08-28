using Supertext.Base.Configuration;

namespace Supertext.Base.Dal.SqlServer.ConnectionThrottling
{
    [ConfigSection("ConnectionThrottleGuard")]
    public class ThrottlingConfig : IConfiguration
    {
        public int MaxCountOfConcurrentSqlConnections { get; set; } = 20;
    }
}