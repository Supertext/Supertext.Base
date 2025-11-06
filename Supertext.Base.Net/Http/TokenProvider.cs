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
        private readonly ITokenCache _tokenCache;
        private readonly ITokenEndpointProvider _tokenEndpointProvider;
        private readonly ILogger<TokenProvider> _logger;

        public TokenProvider(Authentication.Identity identity,
                             IHttpClientFactory httpClientFactory,
                             IFactory<ITracingProvider> tracingProviderFactory,
                             ITokenCache tokenCache,
                             ITokenEndpointProvider tokenEndpointProvider,
                             ILogger<TokenProvider> logger)
        {
            _identity = identity;
            _httpClientFactory = httpClientFactory;
            _tracingProviderFactory = tracingProviderFactory;
            _tokenCache = tokenCache;
            _tokenEndpointProvider = tokenEndpointProvider;
            _logger = logger;
        }

        public async Task<string> RetrieveAccessTokenAsync(string clientId,
                                                           string delegationSub = "",
                                                           string httpClientName = nameof(ITokenProvider),
                                                           AlternativeAuthorityDetails alternativeAuthorityDetails = null,
                                                           IDictionary<string, string> claimsForToken = null)
        {
            var token = await RetrieveTokensAsync(clientId,
                                                  delegationSub,
                                                  httpClientName,
                                                  alternativeAuthorityDetails,
                                                  claimsForToken)
                            .ConfigureAwait(false);


            return token.AccessToken;
        }

        public async Task<TokenResponseDto> RetrieveTokensAsync(string clientId,
                                                                string delegationSub = "",
                                                                string httpClientName = nameof(ITokenProvider),
                                                                AlternativeAuthorityDetails alternativeAuthorityDetails = null,
                                                                IDictionary<string, string> claimsForToken = null)
        {
            var cachedToken = _tokenCache.GetToken(clientId,
                                                   delegationSub,
                                                   httpClientName,
                                                   alternativeAuthorityDetails,
                                                   claimsForToken);

            if (cachedToken.IsSome)
            {
                return cachedToken.Value;
            }

            var token = await RequestTokenAsync(clientId,
                                                delegationSub,
                                                httpClientName,
                                                alternativeAuthorityDetails,
                                                claimsForToken)
                            .ConfigureAwait(false);

            _tokenCache.AddOrUpdateToken(token,
                                         clientId,
                                         delegationSub,
                                         httpClientName,
                                         alternativeAuthorityDetails,
                                         claimsForToken);

            return token;
        }

        public void InvalidateCachedTokens()
        {
            _tokenCache.InvalidateCachedTokens();
        }

        private async Task<TokenResponseDto> RequestTokenAsync(string clientId,
                                                               string delegationSub,
                                                               string httpClientName,
                                                               AlternativeAuthorityDetails alternativeAuthorityDetails,
                                                               IDictionary<string, string> claimsForToken)
        {
            var response = await (String.IsNullOrWhiteSpace(delegationSub)
                                      ? RequestClientCredentialsTokenAsync(clientId,
                                                                           httpClientName,
                                                                           alternativeAuthorityDetails,
                                                                           claimsForToken)
                                      : RequestDelegationTokenAsync(clientId,
                                                                    delegationSub,
                                                                    httpClientName,
                                                                    alternativeAuthorityDetails,
                                                                    claimsForToken)).ConfigureAwait(false);

            return MapTokenResponse(response);
        }

        private async Task<TokenResponse> RequestClientCredentialsTokenAsync(string clientId,
                                                                             string httpClientName = nameof(ITokenProvider),
                                                                             AlternativeAuthorityDetails alternativeAuthorityDetails = null,
                                                                             IDictionary<string, string> claimsForToken = null)
        {
            var client = _httpClientFactory.CreateClient(httpClientName);
            var tokenEndpoint = await _tokenEndpointProvider.GetTokenEndpointAsync(_identity, client, alternativeAuthorityDetails).ConfigureAwait(false);
            var apiResourceDefinition = _identity.GetApiResourceDefinition(clientId);
            var tokenRequest = new ClientCredentialsTokenRequest
                               {
                                   Address = tokenEndpoint,
                                   ClientId = clientId,
                                   ClientSecret = alternativeAuthorityDetails?.ClientSecret ?? apiResourceDefinition.Value.ClientSecret,
                                   Scope = apiResourceDefinition.Value.Scope
                               };

            using (tokenRequest)
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
                                                                      string httpClientName = nameof(ITokenProvider),
                                                                      AlternativeAuthorityDetails alternativeAuthorityDetails = null,
                                                                      IDictionary<string, string> claimsForToken = null)
        {
            var client = _httpClientFactory.CreateClient(httpClientName);
            var tokenEndpoint = await _tokenEndpointProvider.GetTokenEndpointAsync(_identity, client, alternativeAuthorityDetails).ConfigureAwait(false);
            var apiResourceDefinition = _identity.GetApiResourceDefinition(clientId);
            var tokenRequest = new TokenRequest
                               {
                                   Address = tokenEndpoint,
                                   ClientId = clientId,
                                   GrantType = "delegation",
                                   ClientSecret = alternativeAuthorityDetails?.ClientSecret ?? apiResourceDefinition.Value.ClientSecret,
                                   Parameters = { { "sub", sub } }
                               };

            using (tokenRequest)
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