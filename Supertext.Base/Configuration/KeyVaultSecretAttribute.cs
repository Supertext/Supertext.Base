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
    }
}