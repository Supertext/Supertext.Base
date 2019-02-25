using Supertext.Base.Configuration;

namespace Supertext.Base.NetFramework.Configuration.Specs
{
    public class DummyConfig : IConfiguration
    {
        public int SomeInt { get; set; }

        [SettingsKey("someString")]
        public string Value { get; set; }

        public double DoubleValue { get; set; }

        [ConnectionStringName("Supertext")]
        public string ConnectionString { get; set; }
    }
}