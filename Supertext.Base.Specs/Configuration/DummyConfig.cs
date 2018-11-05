using Supertext.Base.Configuration;

namespace Supertext.Base.Specs.Configuration
{
    public class DummyConfig : IConfiguration
    {
        public int SomeInt { get; set; }

        [SettingsKey("someString")]
        public string Value { get; set; }

        public double DoubleValue { get; set; }
    }
}