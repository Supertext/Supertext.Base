using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Extensions;

namespace Supertext.Base.Specs.Extensions
{
    [TestClass]
    public class StringBuilderExtensionsTests
    {
        private System.Text.StringBuilder _testee;

        [TestInitialize]
        public void TestMethodInit()
        {
            _testee = new System.Text.StringBuilder();
            _testee.Append("first-string");
            _testee.Append("second-string");
            _testee.Append("third-string");
            _testee.Append("fourth-string");
        }

        [TestMethod]
        public void IndexOf_Returns_Expected_Index_With_startIndex_Ommitted()
        {
            // Arrange

            // Act
            var result = _testee.IndexOf("second-string");

            // Assert
            result.Should().Be(12);
        }

        [TestMethod]
        public void IndexOf_Returns_Expected_Index_With_startIndex_Specified()
        {
            // Arrange

            // Act
            var result = _testee.IndexOf("second-string", 5);

            // Assert
            result.Should().Be(12);
        }

        [TestMethod]
        public void IndexOf_Returns_Minus1_For_Unrecognised_String()
        {
            // Arrange

            // Act
            var result = _testee.IndexOf("fifth-string");

            // Assert
            result.Should().Be(-1);
        }

        [TestMethod]
        public void IndexOf_Returns_Minus1_For_Recognised_String_Before_startIndex()
        {
            // Arrange

            // Act
            var result = _testee.IndexOf("second-string", 20);

            // Assert
            result.Should().Be(-1);
        }

        [TestMethod]
        public void IndexOf_Returns_Expected_Index_With_startIndex_Ommitted_ignoreCase()
        {
            // Arrange

            // Act
            var result = _testee.IndexOf("SECOND-STRING",
                                    0,
                                    true);

            // Assert
            result.Should().Be(12);
        }

        [TestMethod]
        public void IndexOf_Returns_Expected_Index_With_startIndex_Specified_ignoreCase()
        {
            // Arrange

            // Act
            var result = _testee.IndexOf("SECOND-STRING",
                                    5,
                                    true);

            // Assert
            result.Should().Be(12);
        }

        [TestMethod]
        public void IndexOf_Returns_Minus1_For_Unrecognised_String_ignoreCase()
        {
            // Arrange

            // Act
            var result = _testee.IndexOf("FIFTH-STRING",
                                    0,
                                    true);

            // Assert
            result.Should().Be(-1);
        }

        [TestMethod]
        public void IndexOf_Returns_Minus1_For_Recognised_String_Before_startIndex_ignoreCase()
        {
            // Arrange

            // Act
            var result = _testee.IndexOf("SECOND-STRING",
                                    20,
                                    true);

            // Assert
            result.Should().Be(-1);
        }

        [TestMethod]
        public void Contains_Returns_TRUE_For_Recognised_String()
        {
            // Arrange

            // Act
            var result = _testee.Contains("second-string");

            // Assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void Contains_Returns_FALSE_For_Unrecognised_String()
        {
            // Arrange

            // Act
            var result = _testee.Contains("fifth-string");

            // Assert
            result.Should().Be(false);
        }

        [TestMethod]
        public void Contains_Returns_TRUE_For_Recognised_String_ignoreCase()
        {
            // Arrange

            // Act
            var result = _testee.Contains("SECOND-STRING", true);

            // Assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void Contains_Returns_FALSE_For_Unrecognised_String_ignoreCase()
        {
            // Arrange

            // Act
            var result = _testee.Contains("FIFTH-STRING", true);

            // Assert
            result.Should().Be(false);
        }
    }
}