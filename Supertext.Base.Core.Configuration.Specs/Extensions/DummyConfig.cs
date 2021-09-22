using Supertext.Base.Configuration;

namespace Supertext.Base.Core.Configuration.Specs.Extensions
{
    [ConfigSection("TestConfig")]
    public class DummyConfig : IConfiguration
    {
        public int SomeInt { get; set; }

        public string Value { get; set; }

        public double DoubleValue { get; set; }

        [KeyVaultSecret]
        public string ConnectionString { get; set; }

        [KeyVaultSecret("AuthenticationConnectionString")]
        public string AnotherString { get; set; }
    }
}