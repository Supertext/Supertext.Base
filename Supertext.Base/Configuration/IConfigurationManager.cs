using Supertext.Base.Common;

namespace Supertext.Base.Configuration
{
    public interface IConfigurationManager
    {
        Option<object> GetSettingsValue(string settingsKey);
    }
}