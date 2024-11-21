using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Supertext.Base.Test.Utils.Api
{
    public class TestServerProvider<TStartup> where TStartup : class
    {
        public TestServerProvider()
        {
            Server = new TestServer(new WebHostBuilder().UseTestStartup<TestStartup, TStartup>());
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public TestServer Server { get; }
    }
}
