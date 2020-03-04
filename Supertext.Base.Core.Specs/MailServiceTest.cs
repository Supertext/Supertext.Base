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

        [TestInitialize]
        public void Setup()
        {
            _logger = A.Fake<ILogger<MailService>>();
            _testee = new MailService(_logger);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var boxedObject = RuntimeHelpers.GetObjectValue(_testee);

            _testee.GetType().GetProperty("Message")?.SetValue(boxedObject, "I'm testing the email service.");

            _testee = (MailService)boxedObject;

            _testee.Send("Yasaman", "yasaman@supertext.ch", "Yasaman", "yasaman@supertext.ch");
        }
    }
}
