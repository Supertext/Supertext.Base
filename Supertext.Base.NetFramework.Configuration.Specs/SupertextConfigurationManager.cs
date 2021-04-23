using Autofac;
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
        public void AppSettings_UnavailableAppSetting_ReturnsNull()
        {
            var appSettings = Configuration.SupertextConfigurationManager.AppSettings;
            appSettings["UnavailableKey"].Should().Be(null);
        }

        [TestMethod]
        public void GetAppSetting_ExistentAppSetting_ReturnsValueAsString()
        {
            var appSetting = Configuration.SupertextConfigurationManager.GetAppSetting("AnotherInt");
            appSetting.Should().Be("9712");
        }

        [TestMethod]
        public void GetAppSetting_ExistentAppSettingInFallBackConfigurationManager_ReturnsValueAsString()
        {
            var appSetting = Configuration.SupertextConfigurationManager.GetAppSetting("SomeInt");
            appSetting.Should().Be("4711");
        }

        [TestMethod]
        public void GetAppSetting_UnavailableAppSetting_ReturnsNull()
        {
            var appSetting = Configuration.SupertextConfigurationManager.GetAppSetting("UnavailableKey");
            appSetting.Should().Be(null);
        }
    }
}