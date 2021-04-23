using System.Collections.Specialized;
using System.Configuration;

namespace Supertext.Base.NetFramework.Configuration
{
    public static class SupertextConfigurationManager
    {
        private static NameValueCollection _allSettings;

        public static NameValueCollection AppSettings
        {
            get
            {
                if (_allSettings == null)
                {
                    var appSettings = SupertextConfigurationSection.GetConfig().AppSettings;
                    var e = appSettings.GetEnumerator();
                    _allSettings = new NameValueCollection(appSettings.Count);
                    while (e.MoveNext())
                    {
                        if (e.Current is Setting setting)
                        {
                            _allSettings.Add(setting.Key, setting.Value);
                        }
                    }
                }

                return _allSettings;
            }
        }

        public static string GetAppSetting(string key, bool fallbackToAppSettings = true)
        {
            var appSettings = AppSettings[key];

            return appSettings != null && fallbackToAppSettings
                       ? ConfigurationManager.AppSettings[key]
                       : null;
        }
    }
}