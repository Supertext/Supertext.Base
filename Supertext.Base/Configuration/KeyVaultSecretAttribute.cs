using System;

namespace Supertext.Base.Configuration
{
    /// <summary>
    /// The KeyVaultSecretAttribute indicates that the value for the decorated property should be obtained from the key vault.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class KeyVaultSecretAttribute : Attribute
    {
        public bool UsePropertyValueAsSecretName { get; }

        public string SecretName { get; }

        /// <summary>
        /// Declare a secret name as configured in the key vault. If secret name is null/not declared, the property name will be taken.
        /// </summary>
        /// <param name="secretName"></param>
        public KeyVaultSecretAttribute(string secretName = null)
        {
            SecretName = secretName;
        }

        /// <summary>
        /// Declares that the actual value of the property will be used to fetch the secret from the key vault. If false, the property name will be taken.
        /// </summary>
        /// <param name="usePropertyValueAsSecretName"></param>
        public KeyVaultSecretAttribute(bool usePropertyValueAsSecretName)
        {
            UsePropertyValueAsSecretName = usePropertyValueAsSecretName;
            SecretName = "";
        }
    }
}