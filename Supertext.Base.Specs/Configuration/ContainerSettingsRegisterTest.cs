using Autofac;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Configuration;
using Supertext.Base.NetFramework.Configuration;

namespace Supertext.Base.Specs.Configuration
{
    [TestClass]
    public class ContainerSettingsRegisterTest
    {
        private ContainerBuilder _builder;

        [TestInitialize]
        public void Setup()
        {
            _builder = new ContainerBuilder();
            _builder.RegisterModule<ConfigurationModule>();
        }

        [TestMethod]
        public void RegisterConfigurationsWithAppConfigValues_SettingsAvailable_RegisteredWithConfiguredValues()
        {
            _builder.RegisterConfigurationsWithAppConfigValues(GetType().Assembly);

            var container = _builder.Build();
            var config = container.Resolve<DummyConfig>();

            config.Value.Should().Be("any Value");
            config.SomeInt.Should().Be(4711);
            config.DoubleValue.Should().Be(0);
        }
    }
}