using System;
using Microsoft.Extensions.Logging;
using Supertext.Base.Hosting.Middleware;
using Supertext.Base.Tracing;

namespace Supertext.Base.Hosting.Tracing;

internal class TracingProvider : ITracingProvider, ITracingInitializer
{
    private readonly ILogger<TracingProvider> _logger;
    private const string DigitsFormat = "N";

    public TracingProvider(ILogger<TracingProvider> logger)
    {
        _logger = logger;
    }

    public Guid CorrelationId { get; private set; }

    public string CorrelationIdDigitsFormat => CorrelationId.ToString(DigitsFormat);

    public string CorrelationIdHeaderName => CorrelationIdOptions.DefaultHeader;

    public void SetNewCorrelationId(Guid correlationId)
    {
        _logger.LogInformation($"{nameof(SetNewCorrelationId)} - Correlation id set: {correlationId}");
        CorrelationId = correlationId;
    }
}