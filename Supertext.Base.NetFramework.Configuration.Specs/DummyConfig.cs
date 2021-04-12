using Supertext.Base.Configuration;

namespace Supertext.Base.NetFramework.Configuration.Specs
{
    public class DummyConfig : IConfiguration
    {
        public int AnotherInt { get; set; }

        public int SomeInt { get; set; }

        [SettingsKey("someString")]
        public string Value { get; set; }

        [SettingsKey("AnotherString")]
        public string AnotherValue { get; set; }

        public double DoubleValue { get; set; }

        [ConnectionStringName("Supertext")]
        public string ConnectionString { get; set; }

        [SettingsKey("supertextSectionConfig")]
        public string SupertextSectionConfig { get; set; }

        [KeyVaultSecret("Super-Secret")]
        public string SuperSecret { get; set; }
    }
}