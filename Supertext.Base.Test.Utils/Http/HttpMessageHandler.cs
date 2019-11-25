using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Supertext.Base.Test.Utils.Http
{
    public class HttpMessageHandler : System.Net.Http.HttpMessageHandler
    {
        private readonly IList<UriAndResponse> _urisAndResponses;

        public HttpMessageHandler(UriAndResponse uriAndResponse)
        {
            _urisAndResponses = new[]
                                    {
                                        uriAndResponse
                                    };
        }

        public HttpMessageHandler(IEnumerable<UriAndResponse> urisAndResponses)
        {
            _urisAndResponses = urisAndResponses.ToList();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // for each matching URI check whether there is a non-NotImplented response
            foreach (var uriAndResponse in _urisAndResponses.Where(uri => uri.Uri.AbsoluteUri == request.RequestUri.AbsoluteUri))
            {
                var response = await uriAndResponse.HandleRequest(request, cancellationToken).ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.NotImplemented)
                {
                    return response;
                }
            }

            // if there was no non-NotImplemented response then return a NotImplemented response
            return new HttpResponseMessage(HttpStatusCode.NotImplemented);
        }
    }
}