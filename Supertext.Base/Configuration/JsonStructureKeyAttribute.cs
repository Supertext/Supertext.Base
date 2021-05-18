using System;

namespace Supertext.Base.Configuration
{
    /// <summary>
    /// The <c>JsonStructureKeyAttribute</c> is used to place on Properties of a settings class in interaction with configurations
    /// in web.config or app.config files, where the value represents a JSON structure.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonStructureKeyAttribute : Attribute
    {
        public string AppSettingsKey { get; }

        public JsonStructureKeyAttribute(string appSettingsKey)
        {
            AppSettingsKey = appSettingsKey;
        }
    }
}