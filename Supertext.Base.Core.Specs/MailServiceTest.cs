using System;
using System.IO;
using System.Linq;
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
        private string _dir;
        private DirectoryInfo _testDir;

        [TestInitialize]
        public void Setup()
        {
            var path = Directory.GetCurrentDirectory();
            _dir = String.Concat(path, "\\temp");
            Directory.CreateDirectory(_dir);
            _testDir = new DirectoryInfo(_dir);
            if (!_testDir.Exists)
            {
                _logger.LogError("Path for temporary local email storage is not correct.");
            }

            _logger = A.Fake<ILogger<MailService>>();
            _config = new MailServiceConfig {LocalEmailDirectory = _dir};
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

        [TestCleanup]
        public void TearDown()
        {
            Directory.Delete(_dir, true);
        }
    }
}
