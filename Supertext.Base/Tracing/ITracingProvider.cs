using System;

namespace Supertext.Base.Tracing;

public interface ITracingProvider
{
    Guid CorrelationId { get; }

    string CorrelationIdDigitsFormat { get; }

    string CorrelationIdHeaderName { get; }
}