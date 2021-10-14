using Microsoft.Extensions.Configuration;

namespace Supertext.Base.Resolver.Url
{
    internal class UrlResolver : IUrlResolver
    {
        private readonly IConfiguration _configuration;

        public UrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ResolveUrl(string domain)
        {
            _configuration.ReplacePlaceholders(domain);
        }
    }
}