using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Supertext.Base.Test.Utils.Http
{
    /// <summary>
    /// <para>A class for handling <c>HttpClient.PostAsync</c> requests and conditionally returning a configured response based upon a conditional function.</para>
    /// <para>The type <c>T</c> should correspond to the content being sent via POST or PUT.</para>
    /// </summary>
    public class ContentConditionalUriAndResponse<TBodyContent> : UriAndResponse
    {
        private readonly Func<string, TBodyContent> _jsonDeserialiser;

        /// <summary>
        /// <para>The conditional function which determines whether <see cref="UriAndResponse.HttpResponse"/> should be returned.</para>
        /// <para>The input argument should correspond to the POST or PUT content.</para>
        /// </summary>
        public Func<TBodyContent, bool> RequestChecker { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="UriAndResponse"/> for handling <c>HttpClient</c> requests and returning a configured response based upon a conditional function.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> which should be handled.</param>
        /// <param name="httpResponse">The response to be returned for the specified <see cref="uri"/> argument.</param>
        /// <param name="requestChecker">
        /// <para>The conditional function which determines whether <see cref="UriAndResponse.HttpResponse"/> should be returned.</para>
        /// <para>The input argument should correspond to the POST or PUT content.</para>
        /// </param>
        /// <param name="jsonDeserialiser">A function which deserialises the content of the request body.</param>
        public ContentConditionalUriAndResponse(Uri uri, HttpResponseMessage httpResponse, Func<TBodyContent, bool> requestChecker, Func<string, TBodyContent> jsonDeserialiser) : base(uri, httpResponse)
        {
            _jsonDeserialiser = jsonDeserialiser;
            RequestChecker = requestChecker ?? throw new ArgumentNullException(nameof(requestChecker), $"If no request content checking is required then use {typeof(UriAndResponse).AssemblyQualifiedName} instead of {typeof(ContentConditionalUriAndResponse<TBodyContent>).AssemblyQualifiedName}.");
        }

        internal protected override async Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var permittedMethods = new List<string>
                                       {
                                           "PATCH",
                                           HttpMethod.Post.Method,
                                           HttpMethod.Put.Method
                                       };

            if (!permittedMethods.Contains(request.Method.Method))
            {
                throw new InvalidOperationException($"Invalid HTTP method: {request.Method.Method}. The conditional form of {typeof(ContentConditionalUriAndResponse<T>).AssemblyQualifiedName} can only be used with PATCH, POST or PUT requests.");
            }

            bool expectationMet;

            if (typeof(T) == typeof(IEnumerable<byte>))
            {
                IEnumerable<byte> byteContent = await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                expectationMet = RequestChecker((T) byteContent);
            }
            else
            {
                T content;
                try
                {
                    var strContent = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                    content = _jsonDeserialiser(strContent);
                }
                catch (Exception)
                {
                    throw new Exception($"Unable to read HttpRequestMessage content as {typeof(T).Name}.");
                }

                expectationMet = RequestChecker(content);
            }

            if (!expectationMet)
            {
                return new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }

            return request.RequestUri.PathAndQuery == Uri.PathAndQuery
                       ? HttpResponse
                       : new HttpResponseMessage(HttpStatusCode.NotImplemented);
        }
    }
}