using System;
using Microsoft.Extensions.Logging;
using Supertext.Base.Tracing;

namespace Supertext.Base.Hosting.Tracing;

internal class TracingProvider : ITracingProvider, ITracingInitializer
{
    private readonly ILogger<TracingProvider> _logger;

    public TracingProvider(ILogger<TracingProvider> logger)
    {
        _logger = logger;
    }

    public Guid CorrelationId { get; private set; }

    public void SetNewCorrelationId(Guid correlationId)
    {
        _logger.LogInformation($"{nameof(SetNewCorrelationId)} - Correlation id set: {correlationId}");
        CorrelationId = correlationId;
    }
}