using System;

namespace Supertext.Base.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SettingsKeyAttribute : Attribute
    {
        public string AppSettingsKey { get; }

        public SettingsKeyAttribute(string appSettingsKey)
        {
            AppSettingsKey = appSettingsKey;
        }
    }
}