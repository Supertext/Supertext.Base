using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Common;

namespace Supertext.Base.Tests.Common
{
    [TestClass]
    public class ValidateTest
    {
        [TestMethod]
        public void NotNullOrWhitespace_NullIsGiven_ExceptionIsThrown()
        {
            string value = null;

            Action validationAction = () => Validate.NotNullOrWhitespace(value);

            validationAction.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void NotNullOrWhitespace_WhitespaceIsGiven_ExceptionIsThrown()
        {
            const string value = " ";

            Action validationAction = () => Validate.NotNullOrWhitespace(value);

            validationAction.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void NotNullOrWhitespace_AnyValueIsGiven_IsValid()
        {
            const string value = "anything";

            Action validationAction = () => Validate.NotNullOrWhitespace(value);

            validationAction.Should().NotThrow();
        }
    }
}