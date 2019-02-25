using Autofac;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Dal.SqlServer.Modules;
using Supertext.Base.Dal.SqlServer.Utils;

namespace Supertext.Base.Dal.SqlServer.Specs.Utils
{
    [TestClass]
    public class RetryPolicyProviderTest
    {
        private ContainerBuilder _builder;

        [TestInitialize]
        public void TestInitialize()
        {
            _builder = new ContainerBuilder();
        }

        [TestMethod]
        public void ResolveRetryPolicyProvider_SqlServerModuleIsRegistered_StrategyPolicyProviderIsResolved()
        {
            _builder.RegisterModule<SqlServerModule>();
            var container = _builder.Build();

            var policyProvider = container.Resolve<IRetryPolicyProvider>();

            policyProvider.Should().BeOfType<StrategyPolicyProvider>();
            policyProvider.RetryPolicy.Should().NotBeNull();
        }
    }
}