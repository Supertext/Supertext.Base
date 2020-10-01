namespace Supertext.Base.Authentication
{
    public class ApiResourceDefinition
    {
        public string ClientId { get; set; }
        /// <summary>
        /// Name of the client secret as declared in the key vault. Is needed to read the value from the key vault and set it to ClientSecret.
        /// </summary>
        public string ClientSecretName { get; set; }
        public string ClientSecret { get; set; }

        /// <summary>
        /// Space separated list of the requested scopes
        /// </summary>
        public string Scope { get; set; }
    }
}