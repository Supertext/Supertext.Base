using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Supertext.Base.Test.Utils.Http
{
    /// <summary>
    /// A class for handling multiple <c>HttpClient</c> requests to the same URI and returning a response which is configured for each request.
    /// </summary>
    public class OrderedUriAndResponse : UriAndResponse
    {
        private readonly Lazy<HttpResponseMessage> _fallbackResponse = new Lazy<HttpResponseMessage>(() => new HttpResponseMessage(HttpStatusCode.NotImplemented));
        private int _requestCount;

        /// <summary>
        /// Gets or sets the ordered responses to be returned for the specified <see cref="Uri"/> property.
        /// </summary>
        public IReadOnlyList<HttpResponseMessage> HttpResponses { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="UriAndResponse"/> for handling multiple <c>HttpClient</c> requests and returning a response which is configured for each request.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> which should be handled.</param>
        /// <param name="orderedHttpResponses">An ordered collection of responses to be returned for the specified <see cref="uri"/> argument.</param>
        public OrderedUriAndResponse(Uri uri, IReadOnlyList<HttpResponseMessage> orderedHttpResponses) : base(uri, null)
        {
            HttpResponses = orderedHttpResponses;
        }

        internal protected override Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(request.RequestUri.AbsoluteUri != Uri.AbsoluteUri || HttpResponses.Count < _requestCount + 1
                                       ? _fallbackResponse.Value
                                       : HttpResponses.ElementAt(_requestCount++));
        }
    }
}