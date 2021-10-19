using System;
using Autofac;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Supertext.Base.Core.Configuration.Specs.Extensions
{
    [TestClass]
    public class ContainerSettingsRegisterOverloadTest
    {
        private IHostEnvironment _environment;
        private ContainerBuilder _builder;

        [TestInitialize]
        public void Setup()
        {
            _environment = A.Fake<IHostEnvironment>();
            A.CallTo(() => _environment.ContentRootPath).Returns(AppDomain.CurrentDomain.BaseDirectory);
            A.CallTo(() => _environment.EnvironmentName).Returns("debug");

            _builder = new ContainerBuilder();

            var configurationBuilder = new ConfigurationBuilder()
                                       .SetBasePath(_environment.ContentRootPath)
                                       .AddJsonFile("appsettings.json")
                                       .AddJsonFile($"appsettings.{_environment.EnvironmentName}.json", optional: true)
                                       .AddEnvironmentVariables();
            var configuration = configurationBuilder.Build();

            _builder.RegisterInstance(configuration).As<IConfiguration>().SingleInstance();
        }

        [TestMethod]
        public void RegisterConfigurationsWithAppConfigValues_SettingsAvailable_RegisteredWithConfiguredValues()
        {
            _builder.RegisterAllConfigurationsInAssembly(GetType().Assembly);

            var container = _builder.Build();
            var config = container.Resolve<DummyConfig>();

            config.Value.Should().Be("any Value");
            config.SomeInt.Should().Be(4711);
            config.DoubleValue.Should().Be(34.5);
        }
    }
}