using Supertext.Base.Configuration;

namespace Supertext.Base.Specs.Factory.AttributeComponents
{
    public abstract class ConfigBase : IConfiguration
    {
        public abstract string Url { get; set; }
    }
}