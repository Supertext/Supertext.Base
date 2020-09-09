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

        public async Task<HttpRequestMessage> CreateHttpRequestMessageProtectedWithBearerToken(HttpMethod method,
                                                                                               string requestUri,
                                                                                               HttpContent content,
                                                                                               string clientId)
        {
            var request = new HttpRequestMessage(method, requestUri) { Content = content };
            var token = await RequestClientCredentialsTokenAsync(clientId).ConfigureAwait(false);
            request.SetBearerToken(token);
            return request;
        }

        private async Task<string> RequestClientCredentialsTokenAsync(string clientId)
        {
            var client = _httpClientFactory.CreateClient(nameof(ProtectedHttpRequestMessageFactory));

            var disco = await client.GetDiscoveryDocumentAsync(_identity.Authority).ConfigureAwait(false);
            if (disco.IsError)
            {
                _logger.LogError(disco.Error);
                throw new Exception($"Discovering oidc document on {_identity.Authority} for retrieving token failed: {disco.Error}");
            }

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
    }
}