using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Supertext.Base.Common;
using Supertext.Base.Hosting.Tracing;

namespace Supertext.Base.Hosting.Middleware;

public class CorrelationIdMiddleware
{
    private static readonly string DefaultCorrelationId = Guid.Empty.ToString();
    private static readonly string DefaultCorrelationIdDigitsOnly = DefaultCorrelationId.Replace("-", "");
    private readonly RequestDelegate _next;
    private readonly ISequentialGuidFactory _guidFactory;
    private readonly ICorrelationIdExtractor _correlationIdExtractor;
    private readonly CorrelationIdOptions _options;

    public CorrelationIdMiddleware(RequestDelegate next,
                                   IOptions<CorrelationIdOptions> options,
                                   ISequentialGuidFactory guidFactory,
                                   ICorrelationIdExtractor correlationIdExtractor = null)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _next = next ?? throw new ArgumentNullException(nameof(next));
        _guidFactory = guidFactory;
        _correlationIdExtractor = correlationIdExtractor;

        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, ITracingInitializer tracingInitializer)
    {
        if (_correlationIdExtractor != null && _correlationIdExtractor.IsHandlingItem(context.Items))
        {
            var correlationId = _correlationIdExtractor.Extract(context.Items);
            context.TraceIdentifier = correlationId ?? context.TraceIdentifier;
        }
        else if (context.Request.Headers.TryGetValue(_options.Header, out var correlationId)
            && correlationId.ToString() != DefaultCorrelationId
            && correlationId.ToString() != DefaultCorrelationIdDigitsOnly)
        {
            context.TraceIdentifier = correlationId.ToString();
        }
        else
        {
            var newCorrelationId = _guidFactory.Create().ToString();
            context.TraceIdentifier = newCorrelationId;
            context.Request.Headers.Remove(_options.Header);
            context.Request.Headers.Append(_options.Header, new StringValues(newCorrelationId));
        }
        tracingInitializer.SetNewCorrelationId(Guid.Parse(context.TraceIdentifier));

        if (_options.IncludeInResponse)
        {
            // apply the correlation ID to the response header for client side tracking
            context.Response.OnStarting(() =>
                                        {
                                            context.Response.Headers.Remove(_options.Header);
                                            context.Response.Headers.Append(_options.Header, new StringValues( context.TraceIdentifier ));
                                            return Task.CompletedTask;
                                        });
        }

        await _next(context);
    }
}