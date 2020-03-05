using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Net.Mail;
using FakeItEasy;

namespace Supertext.Base.Net.Specs
{
    [TestClass]
    public class MailServiceTest
    {
        private MailService _testee;
        private ILogger<MailService> _logger;
        private MailServiceConfig _config;

        [TestInitialize]
        public void Setup()
        {
            _logger = A.Fake<ILogger<MailService>>();
            _config = A.Fake<MailServiceConfig>();
            _testee = new MailService(_logger, _config);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var mailServiceBoxedObject = RuntimeHelpers.GetObjectValue(_testee);
            _testee.GetType().GetProperty("Message")?.SetValue(mailServiceBoxedObject, "I'm testing the email service.");
            _testee.GetType().GetProperty("Subject")?.SetValue(mailServiceBoxedObject, "Test");

            var configBoxedObject = RuntimeHelpers.GetObjectValue(_config);
            _config.GetType().GetProperty("SendGridEnabled")?.SetValue(configBoxedObject, true);
            _config.GetType().GetProperty("SendGridHost")?.SetValue(configBoxedObject, "smtp.sendgrid.net");
            _config.GetType().GetProperty("SendGridPassword")?.SetValue(configBoxedObject, "");
            _config.GetType().GetProperty("SendGridUsername")?.SetValue(configBoxedObject, "");

            _testee = (MailService)mailServiceBoxedObject;
            _config = (MailServiceConfig)configBoxedObject;

            _testee.Send("", "", "", "");
        }
    }
}
