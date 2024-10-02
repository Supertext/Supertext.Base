using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Email;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Net.Mail;

namespace Supertext.Base.Net.Specs.Mail
{
    [TestClass]
    public class MailServiceTest
    {
        private MailService _testee;
        private ILogger<IMailService> _logger;
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
            _logger = A.Fake<ILogger<IMailService>>();
            if (!_testDir.Exists)
            {
                _logger.LogError("Path for temporary local email storage is not correct.");
            }

            _config = new MailServiceConfig {LocalEmailDirectory = _dir};
            _testee = new MailService(_logger, _config);

            var to = new PersonInfo("Verifier", "verifier@mail.com");

            var from = new PersonInfo("Tester", "tester@mail.com");

            _mail = new EmailInfo("Test", "Testing the mail service.", from, to);
        }

        [TestMethod]
        public async Task EmailService_SendGridDisabled_EmailShouldBeStoredLocally()
        {
            _config.SendGridEnabled = false;

            await _testee.SendAsync(_mail);

            var directory = new DirectoryInfo(_dir);

            var myFile = directory.GetFiles()
                                  .OrderByDescending(f => f.LastWriteTime)
                                  .First();

            var mailMessage = MailMessage.Load(myFile.FullName);

            mailMessage.From.Address.Should().Be(_mail.From.Email);
        }

        [TestMethod]
        public async Task EmailService_MultipleRecipients_Success()
        {
            var to1 = new PersonInfo("Verifier 1", "verifier1@mail.com");
            var to2 = new PersonInfo("Verifier 2", "verifier2@mail.com");

            var from = new PersonInfo("Tester", "tester@mail.com");

            _mail = new EmailInfo("Test", "Testing the mail service.", from, new List<PersonInfo> { to1, to2 });

            _config.SendGridEnabled = false;

            await _testee.SendAsync(_mail);

            var directory = new DirectoryInfo(_dir);

            var myFile = directory.GetFiles()
                                  .OrderByDescending(f => f.LastWriteTime)
                                  .First();

            var mailMessage = MailMessage.Load(myFile.FullName);
            mailMessage.To.Select(t => t.Address).Should().BeEquivalentTo(new[] { to1.Email, to2.Email });
        }

        [TestCleanup]
        public void TearDown()
        {
            Directory.Delete(_dir, true);
        }
    }
}
