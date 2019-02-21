using System;

namespace Supertext.Base.Configuration
{
    /// <summary>
    /// The ConnectionStringKeyAttribute is used to place on Properties of a settings class in interaction with configurations
    /// in web.config or app.config files.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConnectionStringKeyAttribute : Attribute
    {
        public string ConnectionStringKey { get; }

        public ConnectionStringKeyAttribute(string connectionStringKey)
        {
            ConnectionStringKey = connectionStringKey;
        }
    }
}