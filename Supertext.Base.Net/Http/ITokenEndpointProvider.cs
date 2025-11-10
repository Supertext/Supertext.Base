using System.Net.Http;
using System.Threading.Tasks;
using Supertext.Base.Authentication;

namespace Supertext.Base.Net.Http;

internal interface ITokenEndpointProvider
{
    Task<string> GetTokenEndpointAsync(Authentication.Identity identity, HttpClient client, AlternativeAuthorityDetails alternativeAuthorityDetails = null);
}