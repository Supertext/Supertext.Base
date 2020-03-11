using Supertext.Base.Configuration;

namespace Supertext.Base.Security.NWebSec
{
    public class NWebSecConfig : IConfiguration
    {
        public int StrictTransportSecurityHeaderMaxAge { get; set; }

        public string NWebSecConfigFilePath { get; set; }

        public string NWebSecConfigNamespace { get; set; }

        public string[] AllowedRedirectDestinations { get; set; }
    }
}
