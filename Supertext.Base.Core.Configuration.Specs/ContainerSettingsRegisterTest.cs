using System;
using Autofac;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Supertext.Base.Core.Configuration.Specs
{
    [TestClass]
    public class ContainerSettingsRegisterTest
    {
        private ContainerBuilder _builder;
        private IConfiguration _configuration;
        private IHostingEnvironment _environment;

        [TestInitialize]
        public void Setup()
        {
            _environment = A.Fake<IHostingEnvironment>();
            A.CallTo(() => _environment.ContentRootPath).Returns(AppDomain.CurrentDomain.BaseDirectory);
            A.CallTo(() => _environment.EnvironmentName).Returns("debug");

            _builder = new ContainerBuilder();

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(_environment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{_environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            _configuration = configurationBuilder.Build();
        }

        [TestMethod]
        public void RegisterConfigurationsWithAppConfigValues_SettingsAvailable_RegisteredWithConfiguredValues()
        {
            _builder.RegisterAllConfigurationsInAssembly(_configuration, GetType().Assembly);

            var container = _builder.Build();
            var config = container.Resolve<DummyConfig>();

            config.Value.Should().Be("any Value");
            config.SomeInt.Should().Be(4711);
            config.DoubleValue.Should().Be(34.5);
        }

        [TestMethod]
        public void RegisterConfigurationsWithAppConfigValues_PropertyWithKeyVaultSecret_KeyVaultValueAvailable()
        {
            _builder.RegisterAllConfigurationsInAssembly(_configuration, GetType().Assembly);

            var container = _builder.Build();
            var config = container.Resolve<DummyConfig>();

            config.ConnectionString.Should().Be("some value");
        }

        [TestMethod]
        public void RegisterConfigurationsWithAppConfigValues_PropertyWithExplicitNamedKeyVaultSecret_KeyVaultValueAvailable()
        {
            _builder.RegisterAllConfigurationsInAssembly(_configuration, GetType().Assembly);

            var container = _builder.Build();
            var config = container.Resolve<DummyConfig>();

            config.AnotherString.Should().Be("some other value");
        }
    }
}