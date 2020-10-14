using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Supertext.Base.Authentication;

namespace Supertext.Base.Net.Http
{
    internal class ProtectedHttpRequestMessageFactory : IProtectedHttpRequestMessageFactory
    {
        private readonly Authentication.Identity _identity;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProtectedHttpRequestMessageFactory> _logger;

        public ProtectedHttpRequestMessageFactory(Authentication.Identity identity,
                                                  IHttpClientFactory httpClientFactory,
                                                  ILogger<ProtectedHttpRequestMessageFactory> logger)
        {
            _identity = identity;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<HttpRequestMessage> CreateHttpRequestMessageWithTokenClientCredentials(HttpMethod method,
                                                                                                 string requestUri,
                                                                                                 string clientId,
                                                                                                 HttpContent content = null)
        {
            var request = new HttpRequestMessage(method, requestUri) {Content = content};
            var token = await RequestClientCredentialsTokenAsync(clientId).ConfigureAwait(false);
            request.SetBearerToken(token);
            return request;
        }

        public async Task<HttpRequestMessage> CreateHttpRequestMessageWithTokenDelegation(HttpMethod method,
                                                                                                 string requestUri,
                                                                                                 string clientId,
                                                                                                 string sub,
                                                                                                 HttpContent content = null)
        {
            var request = new HttpRequestMessage(method, requestUri) {Content = content};
            var token = await RequestDelegationTokenAsync(clientId, sub).ConfigureAwait(false);
            request.SetBearerToken(token);
            return request;
        }

        [Obsolete("Use CreateHttpRequestMessageWithTokenClientCredentials instead.")]
        public Task<HttpRequestMessage> CreateHttpRequestMessageProtectedWithBearerToken(HttpMethod method,
                                                                                               string requestUri,
                                                                                               HttpContent content,
                                                                                               string clientId)
        {
            return CreateHttpRequestMessageWithTokenClientCredentials(method,
                                                                      requestUri,
                                                                      clientId,
                                                                      content);
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