using System.Configuration;

namespace Supertext.Base.NetFramework.Configuration
{
    public class AppSettings : ConfigurationElementCollection
    {
        public Setting this[int index]
        {
            get => BaseGet(index) as Setting;
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }

                BaseAdd(index, value);
            }
        }

        public new Setting this[string key]
        {
            get => (Setting) BaseGet(key);
            set
            {
                if (BaseGet(key) != null)
                {
                    BaseRemoveAt(BaseIndexOf(BaseGet(key)));
                }

                BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new Setting();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Setting) element).Key;
        }

        public void Add(string key, string value)
        {
            BaseAdd(new Setting
                        {
                            Key = key,
                            Value = value
                        });
        }

        public void Remove(string key)
        {
            BaseRemove(key);
        }
    }
}