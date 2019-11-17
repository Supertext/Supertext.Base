using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Supertext.Base.Test.Utils.Http
{
    /// <summary>
    /// A class for handling <c>HttpClient</c> requests and returning a configured response.
    /// </summary>
    public class UriAndOrderedResponse : UriAndResponse
    {
        /// <summary>
        /// Creates an instance of <see cref="UriAndResponse"/> for handling <c>HttpClient</c> requests and returning a configured response.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> which should be handled.</param>
        /// <param name="httpResponse">The response to be returned for the specified <see cref="uri"/> argument.</param>
        public UriAndOrderedResponse(Uri uri, HttpResponseMessage httpResponse) : base(uri, httpResponse)
        {
        }

        internal protected virtual Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(request.RequestUri.AbsoluteUri == Uri.AbsoluteUri
                                       ? HttpResponse
                                       : new HttpResponseMessage(HttpStatusCode.NotImplemented));
        }
    }
}