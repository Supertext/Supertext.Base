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
using System.Collections.Concurrent;
using Supertext.Base.Common;

namespace Supertext.Base.Net.Http
{
    internal class TokenProvider : ITokenProvider
    {
        private static readonly ConcurrentDictionary<string, string> TokenEndpointCache = new();
        private static readonly ConcurrentDictionary<string, CachedToken> TokenCache = new();

        private const int TokenExpirationOffsetInSeconds = 120;
        private const int MinValidityForCachingInSeconds = 300;

        private readonly Authentication.Identity _identity;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IFactory<ITracingProvider> _tracingProviderFactory;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<TokenProvider> _logger;

        public TokenProvider(Authentication.Identity identity,
                             IHttpClientFactory httpClientFactory,
                             IFactory<ITracingProvider> tracingProviderFactory,
                             IDateTimeProvider dateTimeProvider,
                             ILogger<TokenProvider> logger)
        {
            _identity = identity;
            _httpClientFactory = httpClientFactory;
            _tracingProviderFactory = tracingProviderFactory;
            _dateTimeProvider = dateTimeProvider;
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
            var cacheKey = BuildCacheKey(clientId,
                                         delegationSub,
                                         httpClientName,
                                         alternativeAuthorityDetails,
                                         claimsForToken);

            if (TokenCache.TryGetValue(cacheKey, out var cachedToken))
            {
                if (cachedToken.ExpiresAt > _dateTimeProvider.UtcNow.AddSeconds(TokenExpirationOffsetInSeconds))
                {
                    return cachedToken.Token;
                }
            }

            var token = await RequestTokenAsync(clientId,
                                                delegationSub,
                                                httpClientName,
                                                alternativeAuthorityDetails,
                                                claimsForToken)
                            .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(token.AccessToken) && token.ExpiresIn <= MinValidityForCachingInSeconds)
            {
                _logger.LogWarning("Token retrieval returned no access token or is about to expire soon. Not caching the token.");
                return token;
            }

            var expiresAt = _dateTimeProvider.UtcNow.AddSeconds(token.ExpiresIn);
            TokenCache[cacheKey] = new TokenProvider.CachedToken
                                    {
                                        Token = token,
                                        ExpiresAt = expiresAt
                                    };

            _logger.LogInformation($"Token for client '{clientId}' cached until {expiresAt} UTC with key '{cacheKey}'.");

            return token;
        }

        public void InvalidateCachedTokens()
        {
            TokenCache.Clear();
            _logger.LogInformation("All cached tokens have been invalidated.");
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
            var tokenEndpoint = await GetTokenEndpointAsync(client, alternativeAuthorityDetails).ConfigureAwait(false);
            var apiResourceDefinition = _identity.GetApiResourceDefinition(clientId);

            using (var tokenRequest = new ClientCredentialsTokenRequest
                                      {
                                          Address = tokenEndpoint,
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
                                                                      string httpClientName = nameof(ITokenProvider),
                                                                      AlternativeAuthorityDetails alternativeAuthorityDetails = null,
                                                                      IDictionary<string, string> claimsForToken = null)
        {
            var client = _httpClientFactory.CreateClient(httpClientName);
            var tokenEndpoint = await GetTokenEndpointAsync(client, alternativeAuthorityDetails).ConfigureAwait(false);
            var apiResourceDefinition = _identity.GetApiResourceDefinition(clientId);

            using (var tokenRequest = new TokenRequest
                                      {
                                          Address = tokenEndpoint,
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

        private async Task<string> GetTokenEndpointAsync(HttpClient client, AlternativeAuthorityDetails alternativeAuthorityDetails = null)
        {
            var cacheKey = alternativeAuthorityDetails?.Authority ?? _identity.Authority;

            if (TokenEndpointCache.TryGetValue(cacheKey, out var cachedTokenEndpoint))
            {
                return cachedTokenEndpoint;
            }

            var discoveryDocument = await GetDiscoveryDocumentAsync(client, alternativeAuthorityDetails).ConfigureAwait(false);

            TokenEndpointCache[cacheKey] = discoveryDocument.TokenEndpoint;

            return discoveryDocument.TokenEndpoint;
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

        private static string BuildCacheKey(string clientId,
                                            string delegationSub,
                                            string httpClientName,
                                            AlternativeAuthorityDetails alternativeAuthorityDetails,
                                            IDictionary<string, string> claimsForToken)
        {
            var claimsKey = claimsForToken != null
                                ? string.Join(";", claimsForToken.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"))
                                : string.Empty;
            var authorityKey = alternativeAuthorityDetails?.Authority ?? string.Empty;
            return $"{clientId}|{delegationSub}|{httpClientName}|{authorityKey}|{claimsKey}";
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

        private class CachedToken
        {
            public TokenResponseDto Token { get; set; }

            public DateTime ExpiresAt { get; set; }
        }
    }
}