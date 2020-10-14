using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Supertext.Base.Authentication
{
    /// <summary>
    /// This factory creates HttpRequestMessages with bearer tokens already set.
    /// The bearer tokens are retrieved from the identity server authority.
    /// </summary>
    /// <remarks>
    /// In appsettings.json Identity with ApiResourceDefinitions must be configured as well as registered
    /// at Autofac like: builder.RegisterIdentityAndApiResourceDefinitions().
    ///
    /// Also register Supertext.Base.Net.NetModule with Autofac.
    /// </remarks>
    public interface IProtectedHttpRequestMessageFactory
    {
        [Obsolete("Use CreateHttpRequestMessageWithTokenClientCredentials instead.")]
        Task<HttpRequestMessage> CreateHttpRequestMessageProtectedWithBearerToken(HttpMethod method,
                                                                                  string requestUri,
                                                                                  HttpContent content,
                                                                                  string clientId);

        /// <summary>
        /// Creates an HttpRequestMessage with obtaining a bearer token with client credentials (client id and client secret).
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestUri"></param>
        /// <param name="clientId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<HttpRequestMessage> CreateHttpRequestMessageWithTokenClientCredentials(HttpMethod method,
                                                                                    string requestUri,
                                                                                    string clientId,
                                                                                    HttpContent content = null);

        /// <summary>
        /// Creates an HttpRequestMessage with obtaining a bearer token with grant type delegation and client credentials (client id and client secret).
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestUri"></param>
        /// <param name="clientId"></param>
        /// <param name="sub"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<HttpRequestMessage> CreateHttpRequestMessageWithTokenDelegation(HttpMethod method,
                                                                             string requestUri,
                                                                             string clientId,
                                                                             string sub,
                                                                             HttpContent content = null);
    }
}