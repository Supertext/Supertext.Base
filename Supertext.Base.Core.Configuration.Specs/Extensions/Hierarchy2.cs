using Supertext.Base.Configuration;

namespace Supertext.Base.Core.Configuration.Specs.Extensions
{
    public class Hierarchy2
    {
        [KeyVaultSecret("PersonSecretName")]
        public string Secret { get; set; }

        public Hierarchy3 Hierarchy3 { get; set; }
    }
}