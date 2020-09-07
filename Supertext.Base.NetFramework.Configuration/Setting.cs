using System.Configuration;

namespace Supertext.Base.NetFramework.Configuration
{
    public class Setting : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true)]
        public string Key
        {
            get => this["key"] as string;
            set => this["key"] = value;
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get => this["value"] as string;
            set => this["value"] = value;
        }
    }
}