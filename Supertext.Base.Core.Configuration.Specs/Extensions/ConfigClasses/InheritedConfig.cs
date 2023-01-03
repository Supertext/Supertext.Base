using Supertext.Base.Configuration;

namespace Supertext.Base.Core.Configuration.Specs.Extensions.ConfigClasses
{
    [ConfigSection("TestConfig")]
    public class InheritedConfig : ConfigBase
    {
        public override string Url { get; set; }
    }
}