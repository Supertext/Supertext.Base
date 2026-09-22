using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Supertext.Base.Test.Utils.Api
{
    public class TestServerProvider<TStartup> where TStartup : class
    {
        public TestServerProvider()
        {
            // Deprecated in net10 (ASPDEPR004/008). Suppressed rather than migrated because
            // TestServerProvider is public API that downstream integration tests build on;
            // moving to WebApplicationBuilder changes host semantics for every consumer.
#pragma warning disable ASPDEPR004, ASPDEPR008
            Server = new TestServer(new WebHostBuilder().UseTestStartup<TestStartup, TStartup>());
#pragma warning restore ASPDEPR004, ASPDEPR008
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public TestServer Server { get; }
    }
}
