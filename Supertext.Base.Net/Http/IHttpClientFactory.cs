using System.Net.Http;

namespace Supertext.Base.Net.Http
{
    public interface IHttpClientFactory
    {
        HttpClient Create();
    }
}