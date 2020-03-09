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
        private EmailInfo _mail;
        const string _dir = @"C:\Tmp\Emails";

        [TestInitialize]
        public void Setup()
        {
            _logger = A.Fake<ILogger<MailService>>();
            _config = new MailServiceConfig();
            _config.LocalEmailDirectory = _dir;
            _testee = new MailService(_logger, _config);

            var to = new PersonInfo
                     {
                         Name = "Verifier",
                         Email = "verifier@mail.com"
                     };

            var from = new PersonInfo
                       {
                           Name = "Tester",
                           Email = "tester@mail.com"
                       };

            _mail = new EmailInfo
                   {
                       Subject = "Test",
                       Message = "Testing the mail service.",
                       To = to,
                       From = from
                   };
        }

        [TestMethod]
        public void EmailService_SendGridDisabled_EmailShouldBeStoredLocally()
        {
            _config.SendGridEnabled = false;

            _testee.Send(_mail);

            var directory = new DirectoryInfo(_dir);

            var myFile = directory.GetFiles()
                                  .OrderByDescending(f => f.LastWriteTime)
                                  .First();

            var mailMessage = MailMessage.Load(myFile.FullName);

            mailMessage.From.Address.Should().Be(_mail.From.Email);
        }
    }
}
