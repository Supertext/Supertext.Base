using System;

namespace Supertext.Base.Hosting.Tracing;

public interface ITracingInitializer
{
    void SetNewCorrelationId(Guid correlationId);
}