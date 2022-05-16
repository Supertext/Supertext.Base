using System;
using System.Linq;
using Supertext.Base.Http;

namespace Supertext.Base.Net.Http
{
    internal class UriBuilder : IUriBuilder, IHostInitializer
    {
        private string _currentDomain;
        private string _currentHost;
        private const string DomainPlaceholder = "{domain}";
        private const string HostPlaceholder = "{host}";

        public Uri CreateAbsoluteUri(string relativeUrl)
        {
            var baseUrl = new Uri($"https://{_currentHost}");
            return new Uri(baseUrl, relativeUrl);
        }

        public Uri ResolveUrl(string urlTemplate)
        {
            return new Uri(urlTemplate.Replace(DomainPlaceholder, _currentDomain).Replace(HostPlaceholder, _currentHost));
        }

        public void AddHost(string host)
        {
            _currentHost = host;
            _currentDomain = String.Join(".", host.Split('.').Reverse().Take(2).Reverse());
        }
    }
}