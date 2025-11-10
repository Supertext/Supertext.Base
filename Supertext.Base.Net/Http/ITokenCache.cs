using System.Collections.Generic;
using Supertext.Base.Authentication;
using Supertext.Base.Common;

namespace Supertext.Base.Net.Http;

internal interface ITokenCache
{
    Option<TokenResponseDto> GetToken(string clientId,
                                      string delegationSub,
                                      string httpClientName,
                                      AlternativeAuthorityDetails alternativeAuthorityDetails,
                                      IDictionary<string, string> claimsForToken);

    void AddOrUpdateToken(TokenResponseDto token,
                          string clientId,
                          string delegationSub,
                          string httpClientName,
                          AlternativeAuthorityDetails alternativeAuthorityDetails,
                          IDictionary<string, string> claimsForToken);

    void InvalidateCachedTokens();
}