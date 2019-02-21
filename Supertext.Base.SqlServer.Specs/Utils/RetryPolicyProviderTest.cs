using Autofac;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.SqlServer.Modules;
using Supertext.Base.SqlServer.Utils;

namespace Supertext.Base.SqlServer.Specs.Utils
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
        public void ResolveIRetryPolicyProvider_SqlServerModuleIsRegistered_DefaultRetryPolicyIsProvided()
        {
            _builder.RegisterModule<SqlServerModule>();
            var container = _builder.Build();

            var policyProvider = container.Resolve<IRetryPolicyProvider>();

            policyProvider.Should().BeOfType<DefaultRetryPolicyProvider>();
            policyProvider.RetryPolicy.Should().NotBeNull();
        }

        [TestMethod]
        public void ResolveIRetryPolicyProvider_SqlServerModuleIsRegisteredAndStratPolPolicyIsConfigured_StratPolRetryPolicyIsProvided()
        {
            _builder.RegisterModule<SqlServerModule>();
            _builder.UseStratPolRetryPolicy();
            var container = _builder.Build();

            var policyProvider = container.Resolve<IRetryPolicyProvider>();

            policyProvider.Should().BeOfType<StratPolPolicyProvider>();
            policyProvider.RetryPolicy.Should().NotBeNull();
        }

        [TestMethod]
        public void ResolveIRetryPolicyProvider_SqlServerModuleIsRegisteredAndStratPolPolicyIsConfiguredWithDifferentOrder_StratPolRetryPolicyIsProvided()
        {
            _builder.UseStratPolRetryPolicy();
            _builder.RegisterModule<SqlServerModule>();

            var container = _builder.Build();

            var policyProvider = container.Resolve<IRetryPolicyProvider>();

            policyProvider.Should().BeOfType<StratPolPolicyProvider>();
            policyProvider.RetryPolicy.Should().NotBeNull();
        }
    }
}