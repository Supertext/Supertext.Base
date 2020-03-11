using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Security.NWebSec;

namespace Supertext.Base.Security.Specs.NWebSec
{
    [TestClass]
    public class HeaderConfigurationParserTest
    {
        private HeaderConfigurationParser _testee;

        [TestInitialize]
        public void TestInitialize()
        {
            var config = new NWebSecConfig
                         {
                            NWebSecConfigNamespace = "{http://nwebsec.com/HttpHeaderSecurityModuleConfig.xsd}",
                            StrictTransportSecurityHeaderMaxAge = 365,
                            NWebSecConfigFilePath = "../../../NWebSec/nwsTestConf.xml",
                            AllowedRedirectDestinations = new string[] { "google.com", "supertext.ch"}
                         };

            _testee = new HeaderConfigurationParser(config);
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