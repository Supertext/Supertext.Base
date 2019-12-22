using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Common;

namespace Supertext.Base.Tests.Common
{
    [TestClass]
    public class WildcardCheckerTest
    {
        private WildcardChecker _testee;

        [TestInitialize]
        public void TestInitialize()
        {
            _testee = new WildcardChecker();
        }

        [TestMethod]
        public void IsPassing_WhenFilterIsValue_ShouldBeTrue()
        {
            // Arrange
            const string testValue = "aValue";
            const string testFilter = testValue;

            // Act
            var result = _testee.IsPassing(testFilter, testValue);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsPassing_WhenFilterHasStartAndEndOfValueWithoutWildcard_ShouldBeFalse()
        {
            // Arrange
            const string testValue = "aValue";
            const string testFilter = "alue";

            // Act
            var result = _testee.IsPassing(testFilter, testValue);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsPassing_WhenFilterHasStartAndEndOfValueWithWildcardInMiddle_ShouldBeTrue()
        {
            // Arrange
            const string testValue = "aValue";
            const string testFilter = "a*lue";

            // Act
            var result = _testee.IsPassing(testFilter, testValue);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsPassing_WhenFilterHasPartsOfValueWithMultipleWildcard_ShouldBeTrue()
        {
            // Arrange
            const string testValue = "aValue";
            const string testFilter = "*Va*e";

            // Act
            var result = _testee.IsPassing(testFilter, testValue);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsPassing_WhenFilterHasOnlyWildcard_ShouldBeTrue()
        {
            // Arrange
            const string testValue = "aValue";
            const string testFilter = "*";

            // Act
            var result = _testee.IsPassing(testFilter, testValue);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsPassing_WhenFilterHasMultipleOptionsAndOneOptionMatches_ShouldBeTrue()
        {
            // Arrange
            const string testValue = "aValue";
            const string testFilter = "some|o*her|*lue";

            // Act
            var result = _testee.IsPassing(testFilter, testValue);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsPassing_WhenFilterHasMultipleOptionsAndNoOptionMatches_ShouldBeFalse()
        {
            // Arrange
            const string testValue = "aValue";
            const string testFilter = "some|o*her|blue";

            // Act
            var result = _testee.IsPassing(testFilter, testValue);

            // Assert
            result.Should().BeFalse();
        }
    }
}