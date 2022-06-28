using Supertext.Base.Configuration;

namespace Supertext.Base.Core.Configuration.Specs.Extensions.ConfigClasses.StartupTests;

[ConfigSection("StartupSettings")]
internal class StartupSettings : IConfiguration
{
    public Cors Cors { get; set; }
}