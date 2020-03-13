using System.Linq;
using Autofac;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Core.Configuration;
using Supertext.Base.IO.Modules;
using Supertext.Base.Security.NWebSec;

namespace Supertext.Base.Security.Specs.NWebSec
{
    [TestClass]
    public class HeaderConfigurationParserTest
    {
        private IHeaderConfigurationParser _testee;
        private ContainerBuilder _builder;
        private IConfigurationRoot _configuration;

        [TestInitialize]
        public void TestInitialize()
        {
            var configurationBuilder = new ConfigurationBuilder()
                                       .AddJsonFile("appsettings.json")
                                       .AddEnvironmentVariables();
            _configuration = configurationBuilder.Build();

            _builder = new ContainerBuilder();
            _builder.RegisterModule<IoModule>();
            _builder.RegisterModule<SecurityModule>();
            _builder.RegisterConfigurationsWithAppConfigValues(_configuration, typeof(NWebSecConfig).Assembly);

            var container = _builder.Build();
            _testee = container.Resolve<IHeaderConfigurationParser>();
        }

        [TestMethod]
        public void Get_CspSettings_From_Parsed_Config_File()
        {
            var cspConfig = _testee.Parse("script-src").ToList();

            cspConfig.Count.Should().Be(1);
            cspConfig.First().Should().Be("www.google.com");
        }
    }
}