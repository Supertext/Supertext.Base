using System;
using Autofac;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Supertext.Base.Core.Configuration.Specs.AzureFunctions
{
    [TestClass]
    public class ContainerSettingsRegisterTest
    {
        private const string SomeUserValue = "some user";
        private ContainerBuilder _builder;
        private IConfiguration _configuration;

        [TestInitialize]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("Username", SomeUserValue);
            Environment.SetEnvironmentVariable("groupshare.username", SomeUserValue);

            _builder = new ContainerBuilder();

            var configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            _configuration = configurationBuilder.Build();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Environment.SetEnvironmentVariable("Username", null);
        }

        [TestMethod]
        public void RegisterConfigurationsWithAppConfigValues_SettingsAvailable_RegisteredWithConfiguredValues()
        {
            Configuration.AzureFunctions.ConfigurationExtension.RegisterConfigurationsWithAppConfigValues(_builder, _configuration, GetType().Assembly);

            var container = _builder.Build();
            var config = container.Resolve<DummyConfig>();

            config.Username.Should().Be(SomeUserValue);
        }

        [TestMethod]
        public void RegisterConfigurationsWithAppConfigValues_SettingsDecoratedWithAttribute_DefaultValuesAreConfigured()
        {
            Configuration.AzureFunctions.ConfigurationExtension.RegisterConfigurationsWithAppConfigValues(_builder, _configuration, GetType().Assembly);

            var container = _builder.Build();
            var config = container.Resolve<DummyConfig>();

            config.GroupshareUserName.Should().Be(SomeUserValue);
        }
    }
}