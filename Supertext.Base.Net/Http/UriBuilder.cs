using System;
using System.Linq;
using Supertext.Base.Http;

namespace Supertext.Base.Net.Http
{
    internal class UriBuilder : IUriBuilder, IDomainInitializer, IHostInitializer
    {
        private string _currentDomain;
        private const string DomainPlaceholder = "{domain}";

        public Uri CreateAbsoluteUri(string relativeUrl)
        {
            var baseUrl = new Uri($"https://{_currentDomain}");
            return new Uri(baseUrl, relativeUrl);
        }

        public Uri ResolveUrl(string urlTemplate)
        {
            return new Uri(urlTemplate.Replace(DomainPlaceholder, _currentDomain));
        }
        
        public void AddDomain(string domain)
        {
            _currentDomain = domain;
        }

        public string GetHost()
        {
            return String.Join(".", _currentDomain.Split('.').Reverse().Take(2).Reverse());
        }
    }
}