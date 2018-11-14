using System;

namespace Supertext.Base.Configuration
{
    /// <summary>
    /// The ConfigSectionAttribute is needed in order to register settings in an ASP.NET Core application
    /// where settings are configured in an appsettings.json file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ConfigSectionAttribute : Attribute
    {
        public string SectionName { get; }

        public ConfigSectionAttribute(string sectionName)
        {
            SectionName = sectionName;
        }
    }
}