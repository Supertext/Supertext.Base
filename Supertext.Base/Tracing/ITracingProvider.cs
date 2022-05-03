using System;

namespace Supertext.Base.Tracing;

public interface ITracingProvider
{
    Guid CorrelationId { get; }
}