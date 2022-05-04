using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Supertext.Base.Hosting.Specs.Middleware.Infrastructure;

internal class CorrelationHandlingApplication : WebApplicationFactory<Startup>
{
    protected override IHostBuilder CreateHostBuilder()
    {
        var builder = Host.CreateDefaultBuilder()
                          .UseSerilog()
                          .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                          .ConfigureWebHostDefaults(x =>
                          {
                              x.UseStartup<Startup>().UseTestServer();
                          });
        return builder;
    }
}