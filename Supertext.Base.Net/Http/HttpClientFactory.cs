using System.Net.Http;

namespace Supertext.Base.Net.Http
{
    internal class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient Create()
        {
            return new HttpClient();
        }
    }
}