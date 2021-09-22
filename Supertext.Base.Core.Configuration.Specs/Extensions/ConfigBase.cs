using Supertext.Base.Configuration;

namespace Supertext.Base.Core.Configuration.Specs.Extensions
{
    public abstract class ConfigBase : IConfiguration
    {
        public abstract string Url { get; set; }
    }
}