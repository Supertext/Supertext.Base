using System.Configuration;

namespace Supertext.Base.NetFramework.Configuration
{
    internal class SupertextConfigurationSection : ConfigurationSection
    {
        public static SupertextConfigurationSection GetConfig()
        {
            return (SupertextConfigurationSection) ConfigurationManager.GetSection("supertextConfiguration") ?? new SupertextConfigurationSection();
        }

        [ConfigurationProperty("appSettings")]
        [ConfigurationCollection(typeof(Setting), AddItemName = "add")]
        public AppSettings AppSettings => this["appSettings"] as AppSettings;
    }
}