using Autofac;
using Supertext.Base.Hosting.Middleware;

namespace Supertext.Base.Hosting.Serilog;

public class HostingSerilogModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<CorrelationIdExtractor>().As<ICorrelationIdExtractor>();
    }
}