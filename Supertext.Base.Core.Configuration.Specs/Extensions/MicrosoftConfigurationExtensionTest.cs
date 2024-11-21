using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Core.Configuration.Specs.Extensions.ConfigClasses;

namespace Supertext.Base.Core.Configuration.Specs.Extensions
{
    [TestClass]
    public class MicrosoftConfigurationExtensionTest
    {
        private IHostEnvironment _environment;
        private IConfigurationRoot _testee;

        [TestInitialize]
        public void TestInitialize()
        {
            _environment = A.Fake<IHostEnvironment>();
            A.CallTo(() => _environment.ContentRootPath).Returns(AppDomain.CurrentDomain.BaseDirectory);
            A.CallTo(() => _environment.EnvironmentName).Returns("Development");

            var configurationBuilder = new ConfigurationBuilder()
                                       .SetBasePath(_environment.ContentRootPath)
                                       .AddJsonFile("appsettings.json")
                                       .AddJsonFile($"appsettings.{_environment.EnvironmentName}.json", optional: true)
                                       .AddEnvironmentVariables();
            _testee = configurationBuilder.Build();
        }

        [TestMethod]
        public void CreateConfiguredSettingsInstance_AppsettingsAreGiven_InstanceIsCreatedAndConfigured()
        {
            var config = _testee.CreateConfiguredSettingsInstance<DummyConfig>();

            config.Value.Should().Be("any Value");
            config.SomeInt.Should().Be(4711);
            config.DoubleValue.Should().Be(34.5);

            config.ConnStrings.Should().NotBeNullOrEmpty()
                                       .And.HaveCount(3)
                                       .And.ContainKey("storageConnStrCom")
                                       .And.ContainKey("storageConnStrDe")
                                       .And.ContainKey("storageConnStrSn");

            config.StorageStrings.ConnStrings.Should().NotBeNullOrEmpty()
                                                      .And.HaveCount(3)
                                                      .And.ContainKey("storageConnStrCom")
                                                      .And.ContainKey("storageConnStrDe")
                                                      .And.ContainKey("storageConnStrSn");
        }

        [TestMethod]
        public void CreateConfiguredSettingsInstance_PropertyWithKeyVaultSecret_KeyVaultValueAvailable()
        {
            var config = _testee.CreateConfiguredSettingsInstance<DummyConfig>();

            config.ConnectionString.Should().Be("some value");
        }

        [TestMethod]
        public void CreateConfiguredSettingsInstance_PropertyWithExplicitNamedKeyVaultSecret_KeyVaultValueAvailable()
        {
            var config = _testee.CreateConfiguredSettingsInstance<DummyConfig>();

            config.AnotherString.Should().Be("some other value");
        }
    }
}