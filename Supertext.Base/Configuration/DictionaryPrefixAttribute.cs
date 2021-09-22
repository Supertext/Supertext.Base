using System;

namespace Supertext.Base.Configuration
{
    /// <summary>
    /// The <c>DictionaryPrefixAttribute</c> is used to place on Properties of a settings class in interaction with configurations
    /// in web.config or app.config files, where the value represents a key prefix on an app-setting.
    /// </summary>
    /// <example>
    /// The following properties in a web.config would be exposed as elements in a Dictionary{string, string}:
    /// &lt;appSettings&gt;
    ///     &lt;add key="examplePrefixBob" value="Switzerland" /&gt;
    ///     &lt;add key="examplePrefixJohn" value="Germany" /&gt;
    /// &lt;/appSettings&gt;
    /// The property would then be decorated: <c>[DictionaryPrefix("examplePrefix")]</c>
    /// </example>
    [AttributeUsage(AttributeTargets.Property)]
    public class DictionaryPrefixAttribute : Attribute
    {
        public string AppSettingPrefix { get; }

        public DictionaryPrefixAttribute(string appSettingPrefix)
        {
            AppSettingPrefix = appSettingPrefix;
        }
    }
}