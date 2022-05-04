using System.Net.Http;
using System.Threading.Tasks;

namespace Supertext.Base.Authentication;

public interface IHttpRequestMessageBuilder
{
    IHttpRequestMessageBuilder Create(HttpMethod method, string requestUri, HttpContent content = null);

    IHttpRequestMessageBuilder UseBearerToken(string clientId, string sub);

    IHttpRequestMessageBuilder UseCorrelationId();

    Task<HttpRequestMessage> BuildAsync();
}