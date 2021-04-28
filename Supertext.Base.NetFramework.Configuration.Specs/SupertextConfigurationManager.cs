using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Supertext.Base.NetFramework.Configuration.Specs
{
    [TestClass]
    public class SupertextConfigurationManager
    {
        [TestMethod]
        public void AppSettings_AppSettingsAvailable_GetsNameValueCollectionWithAllRegisteredSettings()
        {
            var appSettings = Configuration.SupertextConfigurationManager.AppSettings;
            appSettings["AnotherInt"].Should().Be("9712");
            appSettings["AnotherString"].Should().Be("another value");
            appSettings["Super-Secret"].Should().Be("very secret value");
        }

        [TestMethod]
        public void AppSettings_AppSettingUnavailable_ReturnsNull()
        {
            var appSettings = Configuration.SupertextConfigurationManager.AppSettings;
            appSettings["UnavailableKey"].Should().Be(null);
        }

        [TestMethod]
        public void GetAppSetting_AppSettingAvailable_ReturnsValueAsString()
        {
            var appSetting = Configuration.SupertextConfigurationManager.GetAppSetting("AnotherInt");
            appSetting.Should().Be("9712");
        }

        [TestMethod]
        public void GetAppSetting_AppSettingAvailableInFallBackConfigurationManager_ReturnsValueAsString()
        {
            var appSetting = Configuration.SupertextConfigurationManager.GetAppSetting("SomeInt");
            appSetting.Should().Be("4711");
        }

        [TestMethod]
        public void GetAppSetting_AppSettingAvailableInFallBackConfigurationManagerButFallbackSetToFalse_ReturnsNull()
        {
            var appSetting = Configuration.SupertextConfigurationManager.GetAppSetting("SomeInt", false);
            appSetting.Should().Be(null);
        }

        [TestMethod]
        public void GetAppSetting_AppSettingUnavailable_ReturnsNull()
        {
            var appSetting = Configuration.SupertextConfigurationManager.GetAppSetting("UnavailableKey");
            appSetting.Should().Be(null);
        }
    }
}