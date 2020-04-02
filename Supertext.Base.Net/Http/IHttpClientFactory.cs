using System;
using System.Net.Http;

namespace Supertext.Base.Net.Http
{
    [Obsolete("Deprecated soon. Use System.Net.Http.IHttpClientFactory instead")]
    public interface IHttpClientFactory
    {
        HttpClient Create();
    }
}