using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Supertext.Base.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Supertext.Base.Factory;
using Supertext.Base.Tracing;

namespace Supertext.Base.Net.Http
{
    internal class TokenProvider : ITokenProvider
    {
        private readonly Authentication.Identity _identity;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IFactory<ITracingProvider> _tracingProviderFactory;
        private readonly ILogger<TokenProvider> _logger;

        public TokenProvider(Authentication.Identity identity,
                             IHttpClientFactory httpClientFactory,
                             IFactory<ITracingProvider> tracingProviderFactory,
                             ILogger<TokenProvider> logger)
        {
            _identity = identity;
            _httpClientFactory = httpClientFactory;
            _tracingProviderFactory = tracingProviderFactory;
            _logger = logger;
        }

        public async Task<string> RetrieveAccessTokenAsync(string clientId,
                                                           string delegationSub = "",
                                                           AlternativeAuthorityDetails alternativeAuthorityDetails = null,
                                                           IDictionary<string, string> claimsForToken = null)
        {
            if (String.IsNullOrWhiteSpace(delegationSub))
            {
                return (await RequestClientCredentialsTokenAsync(clientId, alternativeAuthorityDetails, claimsForToken).ConfigureAwait(false)).AccessToken;
            }

            return (await RequestDelegationTokenAsync(clientId, delegationSub, alternativeAuthorityDetails, claimsForToken).ConfigureAwait(false)).AccessToken;
        }

        public async Task<TokenResponseDto> RetrieveTokensAsync(string clientId,
                                                                string delegationSub = "",
                                                                AlternativeAuthorityDetails alternativeAuthorityDetails = null,
                                                                IDictionary<string, string> claimsForToken = null)
        {
            if (String.IsNullOrWhiteSpace(delegationSub))
            {
                var credentialsResponse = await RequestClientCredentialsTokenAsync(clientId, alternativeAuthorityDetails, claimsForToken).ConfigureAwait(false);
                return MapTokenResponse(credentialsResponse);
            }

            var response = await RequestDelegationTokenAsync(clientId, delegationSub, alternativeAuthorityDetails, claimsForToken).ConfigureAwait(false);
            return MapTokenResponse(response);
        }

        private async Task<TokenResponse> RequestClientCredentialsTokenAsync(string clientId,
                                                                             AlternativeAuthorityDetails alternativeAuthorityDetails = null,
                                                                             IDictionary<string, string> claimsForToken = null)
        {
            var client = _httpClientFactory.CreateClient(nameof(ITokenProvider));
            var disco = await GetDiscoveryDocumentAsync(client).ConfigureAwait(false);
            var apiResourceDefinition = _identity.GetApiResourceDefinition(clientId);

            using (var tokenRequest = new ClientCredentialsTokenRequest
                                      {
                                          Address = disco.TokenEndpoint,
                                          ClientId = clientId,
                                          ClientSecret = alternativeAuthorityDetails?.ClientSecret ?? apiResourceDefinition.Value.ClientSecret,
                                          Scope = apiResourceDefinition.Value.Scope
                                      })
            {
                AddClaimsForTokenAsParameters(tokenRequest, claimsForToken);
                EnhanceHeaderWithCorrelationId(tokenRequest);
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(tokenRequest).ConfigureAwait(false);

                if (tokenResponse.IsError)
                {
                    var errorMessage = $"Retrieving token for accessing {clientId} failed: {tokenResponse.Error}. Hint: Look in the logfile of Person.Web about further information.";
                    _logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                return tokenResponse;
            }
        }

        private async Task<TokenResponse> RequestDelegationTokenAsync(string clientId,
                                                                      string sub,
                                                                      AlternativeAuthorityDetails alternativeAuthorityDetails = null,
                                                                      IDictionary<string, string> claimsForToken = null)
        {
            var client = _httpClientFactory.CreateClient(nameof(ITokenProvider));
            var disco = await GetDiscoveryDocumentAsync(client, alternativeAuthorityDetails).ConfigureAwait(false);
            var apiResourceDefinition = _identity.GetApiResourceDefinition(clientId);

            using (var tokenRequest = new TokenRequest
                                      {
                                          Address = disco.TokenEndpoint,
                                          ClientId = clientId,
                                          GrantType = "delegation",
                                          ClientSecret = alternativeAuthorityDetails?.ClientSecret ?? apiResourceDefinition.Value.ClientSecret,
                                          Parameters = { { "sub", sub } }
                                      })
            {
                AddClaimsForTokenAsParameters(tokenRequest, claimsForToken);
                EnhanceHeaderWithCorrelationId(tokenRequest);
                var tokenResponse = await client.RequestTokenAsync(tokenRequest).ConfigureAwait(false);

                if (tokenResponse.IsError)
                {
                    var errorMessage = $"Retrieving token for grant type 'delegation' with client id '{clientId}' failed: {tokenResponse.Error}. Hint: Look in the logfile of Person.Web about further information.";
                    _logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                return tokenResponse;
            }
        }

        private void AddClaimsForTokenAsParameters(TokenRequest tokenRequest, IDictionary<string, string> claimsForToken)
        {
            if (claimsForToken != null)
            {
                var prefixedClaimsForToken = claimsForToken.Select(item =>
                                                                       new KeyValuePair<string, string>($"ClaimForToken:{item.Key}", item.Value));
                tokenRequest.Parameters.AddRange(prefixedClaimsForToken);
            }
        }

        private void EnhanceHeaderWithCorrelationId(ProtocolRequest tokenRequest)
        {
            try
            {
                var tracingProvider = _tracingProviderFactory.Create();
                tokenRequest.Headers.Add(tracingProvider.CorrelationIdHeaderName, tracingProvider.CorrelationIdDigitsFormat);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "Exception occurred while trying to retrieve correlation id and adding the http header.");
            }
        }

        private async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(HttpClient client, AlternativeAuthorityDetails alternativeAuthorityDetails = null)
        {
            var authority = alternativeAuthorityDetails?.Authority ?? _identity.Authority;
            var disco = await client.GetDiscoveryDocumentAsync(authority).ConfigureAwait(false);
            if (disco.IsError)
            {
                _logger.LogError(disco.Error);
                throw new Exception($"Discovering oidc document on {authority} for retrieving token failed: {disco.Error}");
            }

            return disco;
        }

        private static TokenResponseDto MapTokenResponse(TokenResponse response)
        {
            return new TokenResponseDto
                   {
                       AccessToken = response.AccessToken,
                       ErrorDescription = response.ErrorDescription,
                       ExpiresIn = response.ExpiresIn,
                       IdentityToken = response.IdentityToken,
                       IssuedTokenType = response.IssuedTokenType,
                       RefreshToken = response.RefreshToken,
                       Scope = response.Scope,
                       TokenType = response.TokenType
                   };
        }
    }
}