using System;
using System.Linq;
using Autofac;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Core.Configuration.Specs.Extensions.ConfigClasses;
using Supertext.Base.Security.NWebSec;

namespace Supertext.Base.Core.Configuration.Specs.Extensions
{
    [TestClass]
    public class ContainerSettingsRegisterTest
    {
        private ContainerBuilder _builder;
        private IConfiguration _configuration;
        private IHostEnvironment _environment;

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
            _configuration = configurationBuilder.Build();
        }

        [TestMethod]
        public void RegisterAllConfigurationsInAssembly_SettingsAvailable_RegisteredWithConfiguredValues()
        {
            _builder.RegisterAllConfigurationsInAssembly(_configuration, GetType().Assembly);

            var container = _builder.Build();
            var config = container.Resolve<DummyConfig>();

            config.Value.Should().Be("any Value");
            config.SomeInt.Should().Be(4711);
            config.DoubleValue.Should().Be(34.5);
        }

        [TestMethod]
        public void RegisterAllConfigurationsInAssembly_KeyVaultSecretContainsValue_ValueIsBeingUsedToFetchSecretFromKeyVault()
        {
            _builder.RegisterAllConfigurationsInAssembly(_configuration, GetType().Assembly);

            var container = _builder.Build();
            var config = container.Resolve<DummyConfig>();

            config.Secret.Should().Be("secret2");
        }

        [TestMethod]
        public void RegisterAllConfigurationsInAssembly_PropertyWithKeyVaultSecret_KeyVaultValueAvailable()
        {
            _builder.RegisterAllConfigurationsInAssembly(_configuration, GetType().Assembly);

            var container = _builder.Build();
            var config = container.Resolve<DummyConfig>();

            config.ConnectionString.Should().Be("some value");
        }

        [TestMethod]
        public void RegisterAllConfigurationsInAssembly_PropertyWithExplicitNamedKeyVaultSecret_KeyVaultValueAvailable()
        {
            _builder.RegisterAllConfigurationsInAssembly(_configuration, GetType().Assembly);

            var container = _builder.Build();
            var config = container.Resolve<DummyConfig>();

            config.AnotherString.Should().Be("some other value");
        }

        [TestMethod]
        public void RegisterAllConfigurationsInAssembly_ConfigWithCascadedInnerConfigsWithKeyVaultSecret_KeyVaultValuesAvailable()
        {
            _builder.RegisterAllConfigurationsInAssembly(_configuration, GetType().Assembly);

            var container = _builder.Build();
            var config = container.Resolve<TopHierarchyConfig>();

            config.Hierarchy2.Secret.Should().Be("secret1");
            config.Hierarchy2.Hierarchy3.Secret.Should().Be("secret2");
        }

        [TestMethod]
        public void RegisterAllConfigurationsInAssembly_NWebSecConfig_CanHandlePrimitiveArrayProperties()
        {
            _builder.RegisterAllConfigurationsInAssembly(_configuration, typeof(NWebSecConfig).Assembly);

            var container = _builder.Build();
            var config = container.Resolve<NWebSecConfig>();

            config.StrictTransportSecurityHeaderMaxAge.Should().Be(365);
        }

        [TestMethod]
        public void RegisterAllConfigurationsInAssembly_DummyConfigWithClients_CanHandleComplexArrays()
        {
            _builder.RegisterAllConfigurationsInAssembly(_configuration, typeof(DummyConfig).Assembly);

            var container = _builder.Build();
            var config = container.Resolve<DummyConfig>();

            config.Clients.Count.Should().Be(1);
            config.Clients.First().Secret.Should().Be("secret1");
        }

        [TestMethod]
        public void RegisterIdentityAndApiResourceDefinitions_TwoApiResourcesAreDefined_ClientSecretsAreAvailable()
        {
            _builder.RegisterIdentityAndApiResourceDefinitions(_configuration);

            var container = _builder.Build();
            var identity = container.Resolve<Authentication.Identity>();

            identity.ApiResourceDefinitions.Count.Should().Be(2);
            var personApiDefinition = identity.GetApiResourceDefinition("Supertext.Person.API");
            personApiDefinition.Value.ClientSecret.Should().Be("secret1");

            var integrationApiDefinition = identity.GetApiResourceDefinition("Supertext.Integration.API");
            integrationApiDefinition.Value.ClientSecret.Should().Be("secret2");

            var noneDefinition = identity.GetApiResourceDefinition("Anything");
            noneDefinition.IsNone.Should().BeTrue();
        }

        [TestMethod]
        public void RegisterIdentityAndApiResourceDefinitions_IdentityIsDefined_AttributesAreAvailable()
        {
            _builder.RegisterIdentityAndApiResourceDefinitions(_configuration);

            var container = _builder.Build();
            var identity = container.Resolve<Authentication.Identity>();

            identity.Authority.Should().Be("Super authority");
        }

        [TestMethod]
        public void RegisterAllConfigurationsInAssembly_ConfigDerivesFormBaseClass_AttributesAreAvailable()
        {
            _builder.RegisterAllConfigurationsInAssembly(_configuration, GetType().Assembly);

            var container = _builder.Build();
            var config = container.Resolve<InheritedConfig>();

            config.Url.Should().Be("localhost");
        }
    }
}