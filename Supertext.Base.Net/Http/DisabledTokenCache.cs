using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Supertext.Base.Authentication;
using Supertext.Base.Common;

namespace Supertext.Base.Net.Http
{
    internal class DisabledTokenCache: ITokenCache
    {
        private readonly ILogger<DisabledTokenCache> _logger;

        public DisabledTokenCache(ILogger<DisabledTokenCache> logger)
        {
            _logger = logger;
        }
        public Option<TokenResponseDto> GetToken(string clientId,
                                                 string delegationSub,
                                                 string httpClientName,
                                                 AlternativeAuthorityDetails alternativeAuthorityDetails,
                                                 IDictionary<string, string> claimsForToken)
        {
            return Option<TokenResponseDto>.None();
        }

        public void AddOrUpdateToken(TokenResponseDto token,
                                     string clientId,
                                     string delegationSub,
                                     string httpClientName,
                                     AlternativeAuthorityDetails alternativeAuthorityDetails,
                                     IDictionary<string, string> claimsForToken)
        {
            _logger.LogDebug("Token not cached because caching is disabled. TokenSettings.EnableTokenCaching must be set to true to enable caching for tokens.");
            // No implementation needed since caching is disabled
        }

        public void InvalidateCachedTokens()
        {
            // No implementation needed since caching is disabled
        }
    }
}
