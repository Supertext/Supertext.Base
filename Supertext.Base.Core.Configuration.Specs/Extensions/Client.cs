using Supertext.Base.Configuration;

namespace Supertext.Base.Core.Configuration.Specs.Extensions
{
    public class Client
    {
        public string Domain { get; set; }

        public string Identity { get; set; }

        [KeyVaultSecret(usePropertyValueAsSecretName: true)]
        public string Secret { get; set; }

        public string StartupUrl { get; set; }
    }
}