using Serilog.Events;
using Supertext.Base.Hosting.Middleware;

namespace Supertext.Base.Hosting.Serilog;

internal class CorrelationIdExtractor : ICorrelationIdExtractor
{
    public string? Extract(object logEventProperty)
    {
        var prop = logEventProperty as LogEventProperty;
        return prop?.Value.ToString().Replace("\"", String.Empty);
    }
}