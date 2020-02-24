using System;

namespace Supertext.Base.Configuration
{
    /// <summary>
    /// The KeyVaultSecretAttribute indicates that the value for the decorated property should be obtained from the key vault.
    /// IMPORTANT: Works currently only with .net core applications!
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class KeyVaultSecretAttribute : Attribute
    {
        public string SecretName { get; }

        /// <summary>
        /// Declare secret name as configured in the key vault. If secret name is null/not declared, the property name will be taken.
        /// </summary>
        /// <param name="secretName"></param>
        public KeyVaultSecretAttribute(string secretName = null)
        {
            SecretName = secretName;
        }
    }
}