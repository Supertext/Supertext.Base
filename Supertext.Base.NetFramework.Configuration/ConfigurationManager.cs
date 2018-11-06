using System;
using System.Linq;
using Supertext.Base.Common;
using Supertext.Base.Configuration;

namespace Supertext.Base.NetFramework.Configuration
{
    internal class ConfigurationManager : IConfigurationManager
    {
        public Option<object> GetSettingsValue(string settingsKey)
        {
            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Any(key => key == settingsKey))
            {
                var value = System.Configuration.ConfigurationManager.AppSettings[settingsKey];
                return Option<object>.Some(value);
            }

            Console.WriteLine($"Key {settingsKey} not available");
            return Option<object>.None();
        }
    }
}