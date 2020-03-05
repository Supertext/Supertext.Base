using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Aspose.Email;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Net.Mail;
using FakeItEasy;
using FluentAssertions;

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
        public void EmailService_SendGridEnabled_EmailShouldBeSentBySendGrid()
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

        [TestMethod]
        public void EmailService_SendGridDisabled_EmailShouldBeStoredLocally()
        {
            var dir = @"C:\Tmp\Emails";
            var mailServiceBoxedObject = RuntimeHelpers.GetObjectValue(_testee);
            _testee.GetType().GetProperty("Message")?.SetValue(mailServiceBoxedObject, "I'm testing the email service.");
            _testee.GetType().GetProperty("Subject")?.SetValue(mailServiceBoxedObject, "Test");

            var configBoxedObject = RuntimeHelpers.GetObjectValue(_config);
            _config.GetType().GetProperty("SendGridEnabled")?.SetValue(configBoxedObject, false);
            _config.GetType().GetProperty("LocalEmailDirectory")?.SetValue(configBoxedObject, dir);

            _testee = (MailService)mailServiceBoxedObject;
            _config = (MailServiceConfig)configBoxedObject;

            _testee.Send("Tester", "tester@mail.com", "Verifier", "verifier@mail.com");

            var directory = new DirectoryInfo(dir);

            var myFile = directory.GetFiles()
                                  .OrderByDescending(f => f.LastWriteTime)
                                  .First();

            var mailMessage = MailMessage.Load(myFile.FullName);
            var body = mailMessage.Body;
            var subject = mailMessage.Subject;
            var from = mailMessage.From.Address;

            body.Should().Equals(_testee.Message);
            subject.Should().Equals(_testee.Subject);
            from.Should().Equals("tester@mail.com");
        }
    }
}
