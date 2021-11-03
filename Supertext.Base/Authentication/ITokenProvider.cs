using System.Collections.Generic;
using System.Threading.Tasks;

namespace Supertext.Base.Authentication
{
    public interface ITokenProvider
    {
        /// <summary>
        /// Retrieving an access token by providing an OpenId Connect ClientId.
        /// Optionally, a delegation subject (PersonId) can be provided for retrieving the claims on behalf of that sub.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="delegationSub"></param>
        /// <param name="alternativeAuthorityDetails"></param>
        /// <param name="claimsForToken">Key value pairs of claim type and value, which should be added to the token.</param>
        /// <returns></returns>
        /// <remarks>
        /// In appsettings.json Identity with ApiResourceDefinitions must be configured as well as registered
        /// at Autofac like: builder.RegisterIdentityAndApiResourceDefinitions().
        ///
        /// Also register Supertext.Base.Net.NetModule with Autofac.
        /// </remarks>
        Task<string> RetrieveAccessTokenAsync(string clientId,
                                              string delegationSub = "",
                                              AlternativeAuthorityDetails alternativeAuthorityDetails = null,
                                              IDictionary<string, string> claimsForToken = null);

        Task<TokenResponseDto> RetrieveTokensAsync(string clientId,
                                                   string delegationSub = "",
                                                   AlternativeAuthorityDetails alternativeAuthorityDetails = null,
                                                   IDictionary<string, string> claimsForToken = null);
    }
}