using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Supertext.Base.Authentication;
using Supertext.Base.Common;
using Supertext.Base.Factory;
using Supertext.Base.Tracing;

namespace Supertext.Base.Net.Http;

internal class HttpRequestMessageBuilder : IHttpRequestMessageBuilder
{
    private static object LockObject = new();
    private readonly ITokenProvider _tokenProvider;
    private readonly ITracingProvider _tracingProvider;
    private readonly IFactory _factory;
    private readonly ILogger<HttpRequestMessageBuilder> _logger;
    private readonly SortedList<int, Func<HttpRequestMessage, Task<HttpRequestMessage>>> _actions = new();

    public HttpRequestMessageBuilder(IFactory factory,
                                     ILogger<HttpRequestMessageBuilder> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    private HttpRequestMessageBuilder(ITokenProvider tokenProvider,
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
        var builder = GetBuilderInstance();
        const int executionOrder = 1;
        builder._actions.Add(executionOrder,
                             _ =>
                             {
                                 var request = new HttpRequestMessage(method, requestUri) { Content = content };
                                 return Task.FromResult(request);
                             });
        return builder;
    }

    public IHttpRequestMessageBuilder UseBearerToken(string clientId, string httpClientName = "", string sub = "")
    {
        Validate.NotNull(clientId, nameof(clientId));
        const int executionOrder = 2;
        var builder = GetBuilderInstance();
        builder._actions.Add(executionOrder,
                     async request =>
                     {
                         var token = await builder._tokenProvider.RetrieveAccessTokenAsync(clientId, sub).ConfigureAwait(false);
                         request.SetBearerToken(token);

                         return request;
                     });

        return builder;
    }

    public IHttpRequestMessageBuilder UseCorrelationId()
    {
        const int executionOrder = 3;
        var builder = GetBuilderInstance();
        builder._actions.Add(executionOrder,
                            request =>
                            {
                                request.Headers.Add(builder._tracingProvider.CorrelationIdHeaderName, builder._tracingProvider.CorrelationIdDigitsFormat);
                                _logger.LogInformation($"HttpRequestMessage created. Url={request.RequestUri?.OriginalString}; CorrelationId={builder._tracingProvider.CorrelationIdDigitsFormat}");
                                return Task.FromResult(request);
                            });

        return builder;
    }

    public async Task<HttpRequestMessage> BuildAsync()
    {
        HttpRequestMessage request = null;
        foreach (var action in _actions)
        {
            request = await action.Value(request).ConfigureAwait(false);
        }

        _actions.Clear();
        return request;
    }

    private HttpRequestMessageBuilder GetBuilderInstance()
    {
        lock (LockObject)
        {
            return _tokenProvider == null
                       ? new HttpRequestMessageBuilder(_factory.Create<ITokenProvider>(), _factory.Create<ITracingProvider>(), _logger)
                       : this;
        }
    }
}