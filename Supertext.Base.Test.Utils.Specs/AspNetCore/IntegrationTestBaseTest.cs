using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Test.Utils.Api;
using Supertext.Base.Test.Utils.AspNetCore;

namespace Supertext.Base.Test.Utils.Specs.AspNetCore
{
    [TestClass]
    public class IntegrationTestBaseTest : IntegrationTestBase
    {
        private Foo _foo;

        protected override void ConfigureWebHost(IWebHostConfigurator webHostConfigurator)
        {
            webHostConfigurator.UseEnvironment()
                .UseUrls("http://localhost:61234")
                .RegisterType(() => _foo)
                .UseStartup<TestStartup>();
        }

        [TestInitialize]
        public void Setup()
        {
            _foo = new Foo { Bar = "bar" };
            StartWebHost(new string[0]); 
        }

        [TestCleanup]
        public void Cleanup()
        {
            StopWebHost();
        }

        [TestMethod]
        public void Resolve_WebHostIsInitialized_DesiredTypeIsResolved()
        {
            var result = Resolve<Foo>();

            result.Should().Be(_foo);
            result.Bar.Should().Be("bar");
        }

        private class Foo
        {
            public string Bar { get; set; }
        }
    }
}
