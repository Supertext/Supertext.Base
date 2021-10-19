
using Supertext.Base.Configuration;

namespace Supertext.Base.Core.Configuration.Specs.Extensions
{
    public class Hierarchy3
    {
        [KeyVaultSecret("IntegrationSecretName")]
        public string Secret { get; set; }
    }
}