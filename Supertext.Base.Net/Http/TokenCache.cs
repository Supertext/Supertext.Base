using Microsoft.Extensions.Logging;
using Supertext.Base.Authentication;
using Supertext.Base.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Supertext.Base.Net.Http
{
    internal class TokenCache : ITokenCache
    {
        private const int TokenExpirationOffsetInSeconds = 120;
        private const int MinValidityForCachingInSeconds = 300;

        private readonly ConcurrentDictionary<string, CachedToken> _tokens = new();
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<TokenCache> _logger;

        public TokenCache(IDateTimeProvider dateTimeProvider,
                          ILogger<TokenCache> logger)
        {
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public Option<TokenResponseDto> GetToken(string clientId,
                                                 string delegationSub,
                                                 string httpClientName,
                                                 AlternativeAuthorityDetails alternativeAuthorityDetails,
                                                 IDictionary<string, string> claimsForToken)
        {
            var cacheKey = BuildCacheKey(clientId,
                                         delegationSub,
                                         httpClientName,
                                         alternativeAuthorityDetails,
                                         claimsForToken);

            if (!_tokens.TryGetValue(cacheKey, out var cachedToken))
            {
                return Option<TokenResponseDto>.None();
            }

            var isTokenStillValid = cachedToken.ExpiresAt > _dateTimeProvider.UtcNow.AddSeconds(TokenExpirationOffsetInSeconds);

            _logger.LogDebug("Token for client '{ClientId}' with key '{CacheKey}' is {ValidityStatus}.",
                                   clientId,
                                   cacheKey,
                                   isTokenStillValid ? "still valid" : "expired");

            return isTokenStillValid ? Option<TokenResponseDto>.Some(cachedToken.Token) : Option<TokenResponseDto>.None();
        }

        public void AddOrUpdateToken(TokenResponseDto token,
                                     string clientId,
                                     string delegationSub,
                                     string httpClientName,
                                     AlternativeAuthorityDetails alternativeAuthorityDetails,
                                     IDictionary<string, string> claimsForToken)
        {
            var cacheKey = BuildCacheKey(clientId,
                                         delegationSub,
                                         httpClientName,
                                         alternativeAuthorityDetails,
                                         claimsForToken);

            if (String.IsNullOrWhiteSpace(token.AccessToken) && token.ExpiresIn <= MinValidityForCachingInSeconds)
            {
                _logger.LogWarning("Token has no access token or is about to expire soon. Not caching the token.");
                return;
            }

            var expiresAt = _dateTimeProvider.UtcNow.AddSeconds(token.ExpiresIn);
            _tokens[cacheKey] = new CachedToken
                                {
                                    Token = token,
                                    ExpiresAt = expiresAt
                                };

            _logger.LogDebug("Token for client '{ClientId}' cached until {ExpiresAt} UTC with key '{CacheKey}'.",
                                   clientId,
                                   expiresAt,
                                   cacheKey);
        }

        public void InvalidateCachedTokens()
        {
            _tokens.Clear();
            _logger.LogInformation("All cached tokens have been invalidated.");
        }

        private static string BuildCacheKey(string clientId,
                                            string delegationSub,
                                            string httpClientName,
                                            AlternativeAuthorityDetails alternativeAuthorityDetails,
                                            IDictionary<string, string> claimsForToken)
        {
            var claimsKey = claimsForToken != null
                                ? String.Join(";", claimsForToken.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"))
                                : String.Empty;
            var authorityKey = alternativeAuthorityDetails?.Authority ?? String.Empty;
            return $"{clientId}|{delegationSub}|{httpClientName}|{authorityKey}|{claimsKey}";
        }

        private class CachedToken
        {
            public TokenResponseDto Token { get; set; }

            public DateTime ExpiresAt { get; set; }
        }
    }
}