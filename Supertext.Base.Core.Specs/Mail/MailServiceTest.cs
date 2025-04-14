using Autofac;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Modules;
using Supertext.Base.Net.Mail;

namespace Supertext.Base.Net.Specs.Mail
{
    [TestClass]
    public class MailServiceTest
    {
        private IContainer _container;

        [TestMethod]
        public void InstantiateFromIoC_WhenSendGridIsDisabled_ReturnsFileSystemMailService()
        {
            SetupIocContainer<ConfigWithSendGridDisabled>();

            var instance = _container.Resolve<IMailService>();

            instance.Should().BeOfType<FileSystemMailService>();
        }

        [TestMethod]
        public void InstantiateFromIoC_WhenSendGridIsEnabled_ReturnsSendGridMailService()
        {
            SetupIocContainer<ConfigWithSendGridEnabled>();

            var instance = _container.Resolve<IMailService>();

            instance.Should().BeOfType<SendGridMailService>();
        }

        private void SetupIocContainer<T>() where T : MailServiceConfig, new()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<BaseModule>();
            builder.RegisterModule<NetModule>();

            var loggerFactory = LoggerFactory.Create(config => config.AddConsole());

            builder.RegisterInstance(loggerFactory)
                   .As<ILoggerFactory>()
                   .SingleInstance();

            builder.RegisterGeneric(typeof(Logger<>))
                   .As(typeof(ILogger<>))
                   .SingleInstance();

            builder.Register(_ => new T()).As<MailServiceConfig>();

            _container = builder.Build();
        }

        private class ConfigWithSendGridDisabled : MailServiceConfig
        {
            public ConfigWithSendGridDisabled()
            {
                LocalEmailDirectory = @"C:\temp\emails";
                SendGridEnabled = false;
                SendGridHost = $"Test {nameof(ConfigWithSendGridDisabled)}.{nameof(SendGridHost)}";
                SendGridPassword = $"Test {nameof(ConfigWithSendGridDisabled)}.{nameof(SendGridPassword)}";
                SendGridUsername = $"Test {nameof(ConfigWithSendGridDisabled)}.{nameof(SendGridUsername)}";
            }
        }

        private class ConfigWithSendGridEnabled : MailServiceConfig
        {
            public ConfigWithSendGridEnabled()
            {
                LocalEmailDirectory = @"C:\temp\emails";
                SendGridEnabled = true;
                SendGridHost = $"Test {nameof(ConfigWithSendGridEnabled)}.{nameof(SendGridHost)}";
                SendGridPassword = $"Test {nameof(ConfigWithSendGridEnabled)}.{nameof(SendGridPassword)}";
                SendGridUsername = $"Test {nameof(ConfigWithSendGridEnabled)}.{nameof(SendGridUsername)}";
            }
        }
    }
}
