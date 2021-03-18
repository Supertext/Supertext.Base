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
        /// <returns></returns>
        /// <remarks>
        /// In appsettings.json Identity with ApiResourceDefinitions must be configured as well as registered
        /// at Autofac like: builder.RegisterIdentityAndApiResourceDefinitions().
        ///
        /// Also register Supertext.Base.Net.NetModule with Autofac.
        /// </remarks>
        Task<string> RetrieveAccessTokenAsync(string clientId, string delegationSub = "");
    }
}