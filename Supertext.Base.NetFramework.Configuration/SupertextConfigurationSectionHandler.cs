using Microsoft.Configuration.ConfigurationBuilders;
using System.Collections.Generic;
using System.Linq;

namespace Supertext.Base.NetFramework.Configuration
{
    public class SupertextConfigurationSectionHandler : SectionHandler<SupertextConfigurationSection>
    {
#pragma warning disable CS0672
        public override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
#pragma warning restore CS0672
        {
            return ConfigSection.AppSettings.OfType<Setting>()
                                .Select(setting => new KeyValuePair<string, object>(setting.Key, setting.Value))
                                .GetEnumerator();
        }

        public override void InsertOrUpdate(string newKey,
                                            string newValue,
                                            string oldKey = null,
                                            object oldItem = null)
        {
            if (newValue == null)
            {
                return;
            }

            if (oldKey != null)
            {
                ConfigSection.AppSettings.Remove(oldKey);
            }

            ConfigSection.AppSettings.Remove(newKey);
            ConfigSection.AppSettings.Add(newKey, newValue);
        }
    }
}