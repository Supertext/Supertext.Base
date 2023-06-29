using Supertext.Base.Configuration;

namespace Supertext.Base.Dal.SqlServer.Tests.Utils
{
    [ConfigSection("TestConfig")]
    public class TestConfig : IConfiguration
    {
        [KeyVaultSecret]
        public string ConnectionString { get; set; }
    }
}