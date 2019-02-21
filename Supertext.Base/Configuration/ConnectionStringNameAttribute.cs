using System;

namespace Supertext.Base.Configuration
{
    /// <summary>
    /// The ConnectionStringNameAttribute is used to place on Properties of a settings class in interaction with configurations
    /// in web.config or app.config files.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConnectionStringNameAttribute : Attribute
    {
        public string ConnectionStringName { get; }

        public ConnectionStringNameAttribute(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }
    }
}