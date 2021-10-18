using System;
using Supertext.Base.Http;

namespace Supertext.Base.Net.Http
{
    internal class UriBuilder : IUriBuilder, IDomainInitializer
    {
        private string _currentDomain;
        private const string DomainPlaceholder = "{domain}";

        public Uri CreateAbsoluteUri(string relativeUrl)
        {
            return new Uri($"https://{_currentDomain}/{relativeUrl}");
        }

        public Uri ResolveUrl(string urlTemplate)
        {
            return new Uri(urlTemplate.Replace(DomainPlaceholder, _currentDomain));
        }
        
        public void AddDomain(string domain)
        {
            _currentDomain = domain;
        }
    }
}