using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Common;

namespace Supertext.Base.Test.Common
{
    [TestClass]
    public class OptionTest
    {
        [TestMethod]
        public void IsNone_OptionIsNone_IsNone()
        {
            var option = Option<string>.None();

            option.IsNone.Should().BeTrue();
        }

        [TestMethod]
        public void IsSome_OptionWithSome_IsSome()
        {
            var option = Option<string>.Some("anything");

            option.IsSome.Should().BeTrue();
        }

        [TestMethod]
        public void IsSome_OptionWithSomeNull_ExceptionWillBeThrown()
        {
            this.Invoking(x => Option<string>.Some(null)).Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void IsSome_OptionWithSome_HasValue()
        {
            var anything = "anything";
            var option = Option<string>.Some(anything);

            option.Value.Should().Be(anything);
        }
    }
}