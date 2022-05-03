using System.Net.Http;
using System.Threading.Tasks;

namespace Supertext.Base.Authentication;

public interface IHttpRequestMessageBuilder
{
    IHttpRequestMessageBuilder Create(HttpMethod method, string requestUri, HttpContent content = null);

    IHttpRequestMessageBuilder UseBearerTokenWithClientCredentials(string clientId);

    IHttpRequestMessageBuilder UseBearerTokenWithDelegation(string clientId,
                                                            string sub);

    IHttpRequestMessageBuilder UseCorrelationId();

    Task<HttpRequestMessage> BuildAsync();
}