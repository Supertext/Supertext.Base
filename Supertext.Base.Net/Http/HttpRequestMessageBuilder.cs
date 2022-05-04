using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Supertext.Base.Authentication;
using Supertext.Base.Common;
using Supertext.Base.Tracing;

namespace Supertext.Base.Net.Http;

internal class HttpRequestMessageBuilder : IHttpRequestMessageBuilder
{
    private readonly ITokenProvider _tokenProvider;
    private readonly ITracingProvider _tracingProvider;
    private readonly ILogger<HttpRequestMessageBuilder> _logger;
    private readonly SortedList<int, Func<HttpRequestMessage, Task<HttpRequestMessage>>> _actions = new();

    public HttpRequestMessageBuilder(ITokenProvider tokenProvider,
                                     ITracingProvider tracingProvider,
                                     ILogger<HttpRequestMessageBuilder> logger)
    {
        _tokenProvider = tokenProvider;
        _tracingProvider = tracingProvider;
        _logger = logger;
    }

    public IHttpRequestMessageBuilder Create(HttpMethod method, string requestUri, HttpContent content = null)
    {
        Validate.NotNull(method, nameof(method));
        Validate.NotNull(requestUri, nameof(requestUri));
        const int executionOrder = 1;
        _actions.Add(executionOrder,
                     _ =>
                     {
                         var request = new HttpRequestMessage(method, requestUri) { Content = content };
                         return Task.FromResult(request);
                     });
        return this;
    }

    public IHttpRequestMessageBuilder UseBearerToken(string clientId, string sub = "")
    {
        Validate.NotNull(clientId, nameof(clientId));
        const int executionOrder = 2;
        _actions.Add(executionOrder,
                     async request =>
                     {
                         var token = await _tokenProvider.RetrieveAccessTokenAsync(clientId, sub).ConfigureAwait(false);
                         request.SetBearerToken(token);

                         return request;
                     });

        return this;
    }

    public IHttpRequestMessageBuilder UseCorrelationId()
    {
        const int executionOrder = 3;
        _actions.Add(executionOrder,
                     request =>
                     {
                         request.Headers.Add(_tracingProvider.CorrelationIdHeaderName, _tracingProvider.CorrelationIdDigitsFormat);
                         _logger.LogInformation($"HttpRequestMessage created. Url={request.RequestUri?.AbsolutePath}; CorrelationId={_tracingProvider.CorrelationIdDigitsFormat}");
                         return Task.FromResult(request);
                     });

        return this;
    }

    public async Task<HttpRequestMessage> BuildAsync()
    {
        HttpRequestMessage request = null;
        foreach (var action in _actions)
        {
            request = await action.Value(request).ConfigureAwait(false);
        }

        return request;
    }
}