using Supertext.Base.Configuration;
using Supertext.Base.Factory;

namespace Supertext.Base.Specs.Factory.AttributeComponents
{
    [ConfigSection("TestConfig")]
    [ComponentKey("test")]
    public class InheritedConfig : ConfigBase
    {
        public override string Url { get; set; }
    }
}