using System.Net.Http;

namespace Supertext.Base.Net.Http
{
#pragma warning disable 618 // => must be removed, when Supertext.Base.Http.IHttpClientFactory is removed
    internal class HttpClientFactory : IHttpClientFactory
#pragma warning restore 618
    {
        public HttpClient Create()
        {
            return new HttpClient();
        }
    }
}