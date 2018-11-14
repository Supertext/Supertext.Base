using System;

namespace Supertext.Base.Configuration
{
    /// <summary>
    /// The SettingsKeyAttribute is used to place on Properties of a settings class in interaction with configurations
    /// in web.config or app.config files.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SettingsKeyAttribute : Attribute
    {
        public string AppSettingsKey { get; }

        public SettingsKeyAttribute(string appSettingsKey)
        {
            AppSettingsKey = appSettingsKey;
        }
    }
}