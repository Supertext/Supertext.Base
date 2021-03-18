using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Supertext.Base.Authentication;

namespace Supertext.Base.Net.Http
{
    internal class ProtectedHttpRequestMessageFactory : IProtectedHttpRequestMessageFactory
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly ILogger<ProtectedHttpRequestMessageFactory> _logger;

        public ProtectedHttpRequestMessageFactory(ITokenProvider tokenProvider,
                                                  ILogger<ProtectedHttpRequestMessageFactory> logger)
        {
            _tokenProvider = tokenProvider;
            _logger = logger;
        }

        public async Task<HttpRequestMessage> CreateHttpRequestMessageWithTokenClientCredentials(HttpMethod method,
                                                                                                 string requestUri,
                                                                                                 string clientId,
                                                                                                 HttpContent content = null)
        {
            var request = new HttpRequestMessage(method, requestUri) {Content = content};
            var token = await _tokenProvider.RetrieveAccessTokenAsync(clientId).ConfigureAwait(false);
            request.SetBearerToken(token);
            return request;
        }

        public async Task<HttpRequestMessage> CreateHttpRequestMessageWithTokenDelegation(HttpMethod method,
                                                                                                 string requestUri,
                                                                                                 string clientId,
                                                                                                 string sub,
                                                                                                 HttpContent content = null)
        {
            var request = new HttpRequestMessage(method, requestUri) {Content = content};
            var token = await _tokenProvider.RetrieveAccessTokenAsync(clientId, sub).ConfigureAwait(false);
            request.SetBearerToken(token);
            return request;
        }
    }
}