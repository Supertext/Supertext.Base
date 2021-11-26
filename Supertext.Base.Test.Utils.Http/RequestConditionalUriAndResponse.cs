using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Supertext.Base.Test.Utils.Http
{
    /// <summary>
    /// A class for handling <c>HttpClient.PostAsync</c> requests and conditionally returning a configured response based upon a conditional function.
    /// </summary>
    public class RequestConditionalUriAndResponse : UriAndResponse
    {
        /// <summary>
        /// <para>The conditional function which determines whether <see cref="UriAndResponse.HttpResponse"/> should be returned based upon properties of the <c>HttpRequestMessage</c>.</para>
        /// </summary>
        public Func<HttpRequestMessage, bool> RequestChecker { get; set; }

        /// <summary>
        /// <para>The conditional async function which determines whether <see cref="UriAndResponse.HttpResponse"/> should be returned based upon properties of the <c>HttpRequestMessage</c>.</para>
        /// </summary>
        public Func<HttpRequestMessage, Task<bool>> AsyncRequestChecker { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="UriAndResponse"/> for handling <c>HttpClient</c> requests and returning a configured response based upon a conditional function.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> which should be handled.</param>
        /// <param name="httpResponse">The response to be returned for the specified <see cref="uri"/> argument.</param>
        /// <param name="requestChecker">
        /// The conditional function which determines whether <see cref="UriAndResponse.HttpResponse"/> should be returned based upon properties of the <c>HttpRequestMessage</c>.
        /// </param>
        public RequestConditionalUriAndResponse(Uri uri, HttpResponseMessage httpResponse, Func<HttpRequestMessage, bool> requestChecker) : base(uri, httpResponse)
        {
            RequestChecker = requestChecker ?? throw new ArgumentNullException(nameof(requestChecker), $"If no request checking is required then use Supertext.Base.Test.Utils.Http.UriAndResponse instead of Supertext.Base.Test.Utils.Http.{nameof(RequestConditionalUriAndResponse)}.");
        }

        /// <summary>
        /// Creates an instance of <see cref="UriAndResponse"/> for handling <c>HttpClient</c> requests and returning a configured response based upon a conditional async function.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> which should be handled.</param>
        /// <param name="httpResponse">The response to be returned for the specified <see cref="uri"/> argument.</param>
        /// <param name="asyncRequestChecker">
        /// The conditional async function which determines whether <see cref="UriAndResponse.HttpResponse"/> should be returned based upon properties of the <c>HttpRequestMessage</c>.
        /// </param>
        public RequestConditionalUriAndResponse(Uri uri, HttpResponseMessage httpResponse, Func<HttpRequestMessage, Task<bool>> asyncRequestChecker) : base(uri, httpResponse)
        {
            AsyncRequestChecker = asyncRequestChecker ?? throw new ArgumentNullException(nameof(asyncRequestChecker), $"If no request checking is required then use Supertext.Base.Test.Utils.Http.UriAndResponse instead of Supertext.Base.Test.Utils.Http.{nameof(RequestConditionalUriAndResponse)}.");
        }

        internal protected override async Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (RequestChecker != null && !RequestChecker(request)
                || AsyncRequestChecker != null && !await AsyncRequestChecker(request))

            {
                return new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }

            return request.RequestUri.PathAndQuery == Uri.PathAndQuery
                       ? HttpResponse
                       : new HttpResponseMessage(HttpStatusCode.NotImplemented);
        }
    }
}