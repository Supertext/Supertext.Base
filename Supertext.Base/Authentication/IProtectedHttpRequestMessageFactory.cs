using System.Net.Http;
using System.Threading.Tasks;

namespace Supertext.Base.Authentication
{
    /// <summary>
    /// Provides an HttpRequestMessage with bearer token already set for given client.
    /// </summary>
    /// <remarks>
    /// In appsettings.json Identity with ApiResourceDefinitions must be configured as well as registered
    /// at Autofac like: builder.RegisterIdentityAndApiResourceDefinitions().
    ///
    /// Also register Supertext.Base.Net.NetModule with Autofac.
    /// </remarks>
    public interface IProtectedHttpRequestMessageFactory
    {
        Task<HttpRequestMessage> CreateHttpRequestMessageProtectedWithBearerToken(HttpMethod method,
                                                                                  string requestUri,
                                                                                  HttpContent content,
                                                                                  string clientId);
    }
}