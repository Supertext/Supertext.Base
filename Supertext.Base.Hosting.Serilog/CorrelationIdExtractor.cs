using Serilog.Events;
using Supertext.Base.Hosting.Middleware;

namespace Supertext.Base.Hosting.Serilog;

internal class CorrelationIdExtractor : ICorrelationIdExtractor
{
    public string? Extract(IDictionary<object, object?> contextItems)
    {
        if (contextItems.TryGetValue("Serilog_CorrelationId", out var logEventProperty)
            && logEventProperty != null)
        {
            var prop = logEventProperty as LogEventProperty;
            return prop?.Value.ToString().Replace("\"", String.Empty);
        }

        return null;
    }

    public bool IsHandlingItem(IDictionary<object, object> contextItems)
    {
        return contextItems.ContainsKey("Serilog_CorrelationId");
    }
}