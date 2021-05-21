using Autofac;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Supertext.Base.NetFramework.Configuration.Specs
{
    [TestClass]
    public class ContainerSettingsRegisterTest
    {
        private ContainerBuilder _builder;

        [TestInitialize]
        public void Setup()
        {
            _builder = new ContainerBuilder();
        }

        [TestMethod]
        public void RegisterConfigurationsWithAppConfigValues_SettingsAvailable_RegisteredWithConfiguredValues()
        {
            _builder.RegisterConfigurationsWithAppConfigValues(GetType().Assembly);

            var container = _builder.Build();
            var config = container.Resolve<DummyConfig>();

            config.Value.Should().Be("any Value");
            config.AnotherValue.Should().Be("another value");
            config.SomeInt.Should().Be(4711);
            config.AnotherInt.Should().Be(9712);
            config.DoubleValue.Should().Be(0);
            config.ConnectionString.Should().Be("bla");
            config.SuperSecret.Should().Be("very secret value");

            config.JsonWithStrings.Should().NotBeNull();
            config.JsonWithStrings.Should().BeAssignableTo<IEnumerable<KeyValuePair<string, string>>>();
            config.JsonWithStrings.Count().Should().Be(2);
            config.JsonWithStrings.SingleOrDefault(kvp => kvp.Key == "Prop1").Should().NotBeNull();
            config.JsonWithStrings.Single(kvp => kvp.Key == "Prop1").Value.Should().Be("string 1");
            config.JsonWithStrings.SingleOrDefault(kvp => kvp.Key == "Prop2").Should().NotBeNull();
            config.JsonWithStrings.Single(kvp => kvp.Key == "Prop2").Value.Should().Be("string 2");

            config.JsonWithInts.Should().NotBeNull();
            config.JsonWithInts.Should().BeAssignableTo<IEnumerable<KeyValuePair<string, int>>>();
            config.JsonWithInts.Count().Should().Be(2);
            config.JsonWithInts.SingleOrDefault(kvp => kvp.Key == "Prop1").Should().NotBeNull();
            config.JsonWithInts.Single(kvp => kvp.Key == "Prop1").Value.Should().Be(19);
            config.JsonWithInts.SingleOrDefault(kvp => kvp.Key == "Prop2").Should().NotBeNull();
            config.JsonWithInts.Single(kvp => kvp.Key == "Prop2").Value.Should().Be(20);
        }
    }
}