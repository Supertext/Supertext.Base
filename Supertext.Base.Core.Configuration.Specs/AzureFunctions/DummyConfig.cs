using Supertext.Base.Configuration;

namespace Supertext.Base.Core.Configuration.Specs.AzureFunctions
{
    public class DummyConfig : IConfiguration
    {
        public string Username { get; set; }

        [SettingsKey("groupshare.username")]
        public string GroupshareUserName { get; set; }
    }
}