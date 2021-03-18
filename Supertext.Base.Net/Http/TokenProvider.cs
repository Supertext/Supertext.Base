using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Supertext.Base.Authentication;

namespace Supertext.Base.Net.Http
{
    internal class TokenProvider : ITokenProvider
    {
        private readonly Authentication.Identity _identity;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TokenProvider> _logger;

        public TokenProvider(Authentication.Identity identity,
                             IHttpClientFactory httpClientFactory,
                             ILogger<TokenProvider> logger)
        {
            _identity = identity;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public Task<string> RetrieveAccessTokenAsync(string clientId, string delegationSub = "")
        {
            if (String.IsNullOrWhiteSpace(delegationSub))
            {
                return RequestClientCredentialsTokenAsync(clientId);
            }

            return RequestDelegationTokenAsync(clientId, delegationSub);
        }

        private async Task<string> RequestClientCredentialsTokenAsync(string clientId)
        {
            var client = _httpClientFactory.CreateClient(nameof(ProtectedHttpRequestMessageFactory));
            var disco = await GetDiscoveryDocument(client);
            var apiResourceDefinition = _identity.GetApiResourceDefinition(clientId);

            using (var tokenRequest = new ClientCredentialsTokenRequest
                                      {
                                          Address = disco.TokenEndpoint,
                                          ClientId = clientId,
                                          ClientSecret = apiResourceDefinition.Value.ClientSecret,
                                          Scope = apiResourceDefinition.Value.Scope
                                      })
            {
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(tokenRequest).ConfigureAwait(false);

                if (tokenResponse.IsError)
                {
                    var errorMessage = $"Retrieving token for accessing {clientId} failed: {tokenResponse.Error}. Hint: Look in the logfile of Person.Web about further information.";
                    _logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                return tokenResponse.AccessToken;
            }
        }

        private async Task<string> RequestDelegationTokenAsync(string clientId, string sub)
        {
            var client = _httpClientFactory.CreateClient(nameof(ProtectedHttpRequestMessageFactory));
            var disco = await GetDiscoveryDocument(client);
            var apiResourceDefinition = _identity.GetApiResourceDefinition(clientId);

            using (var tokenRequest = new TokenRequest
                                      {
                                          Address = disco.TokenEndpoint,
                                          ClientId = clientId,
                                          GrantType = "delegation",
                                          ClientSecret = apiResourceDefinition.Value.ClientSecret,
                                          Parameters = { { "sub", sub } }
                                      })
            {
                var tokenResponse = await client.RequestTokenAsync(tokenRequest).ConfigureAwait(false);

                if (tokenResponse.IsError)
                {
                    var errorMessage = $"Retrieving token for grant type 'delegation' with client id '{clientId}' failed: {tokenResponse.Error}. Hint: Look in the logfile of Person.Web about further information.";
                    _logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                return tokenResponse.AccessToken;
            }
        }

        private async Task<DiscoveryDocumentResponse> GetDiscoveryDocument(HttpClient client)
        {
            var disco = await client.GetDiscoveryDocumentAsync(_identity.Authority).ConfigureAwait(false);
            if (disco.IsError)
            {
                _logger.LogError(disco.Error);
                throw new Exception($"Discovering oidc document on {_identity.Authority} for retrieving token failed: {disco.Error}");
            }

            return disco;
        }
    }
}