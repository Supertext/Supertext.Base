using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Supertext.Base.Authentication;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Supertext.Base.Net.Http
{
    internal class TokenEndpointProvider : ITokenEndpointProvider
    {
        private readonly ConcurrentDictionary<string, string> _authorityTokenEndpoints = new();
        private readonly ILogger<TokenEndpointProvider> _logger;

        public TokenEndpointProvider(ILogger<TokenEndpointProvider> logger)
        {
            _logger = logger;
        }

        public async Task<string> GetTokenEndpointAsync(Authentication.Identity identity, HttpClient client, AlternativeAuthorityDetails alternativeAuthorityDetails = null)
        {
            var authority = alternativeAuthorityDetails?.Authority ?? identity.Authority;

            if (_authorityTokenEndpoints.TryGetValue(authority, out var cachedTokenEndpoint))
            {
                return cachedTokenEndpoint;
            }

            var discoveryDocument = await GetDiscoveryDocumentAsync(authority, client).ConfigureAwait(false);

            _authorityTokenEndpoints[authority] = discoveryDocument.TokenEndpoint;

            return discoveryDocument.TokenEndpoint;
        }

        private async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(string authority, HttpClient client)
        {
            var disco = await client.GetDiscoveryDocumentAsync(authority).ConfigureAwait(false);
            if (disco.IsError)
            {
                _logger.LogError(disco.Error);
                throw new Exception($"Discovering oidc document on {authority} for retrieving token failed: {disco.Error}");
            }

            return disco;
        }
    }
}