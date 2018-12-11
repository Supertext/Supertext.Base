using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Extensions;
using System;


namespace Supertext.Base.Test.Extensions
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void ReplaceAt_Inserts_String_When_length_Is_0()
        {
            // Arrange
            var str = "this is a string extension unit test";

            // Act
            str = str.ReplaceAt(5,
                                0,
                                "INSERT");

            // Assert
            str.Should().Be("this INSERTis a string extension unit test");
        }


        [TestMethod]
        public void ReplaceAt_Replaces_String()
        {
            // Arrange
            var str = "this is a string extension unit test";
            var insertStr = "INSERT";

            // Act
            str = str.ReplaceAt(5,
                                insertStr.Length,
                                insertStr);

            // Assert
            str.Should().Be("this INSERTtring extension unit test");
        }


        [TestMethod]
        public void ReplaceAt_Returns_Longer_String_When_Insert_String_Exceeds_Current_Length()
        {
            // Arrange
            var str = "this is a string extension unit test";
            var insertStr = str.Clone().ToString();

            // Act
            str = str.ReplaceAt(5,
                                str.Length,
                                insertStr);

            // Assert
            str.Should().Be("this this is a string extension unit test");
        }


        [TestMethod]
        public void ReplaceAt_Throws_Expected_Exception_When_length_Is_Too_Large()
        {
            // Arrange
            var str = "this is a string extension unit test";

            // Act
            try
            {
                str.ReplaceAt(5,
                              str.Length + 1,
                              "INSERT");
            }
            catch (ArgumentException exception) when (exception.ParamName == "length")
            {
                return;
            }

            // Assert
            Assert.Fail("The expected exception was not thrown.");
        }


        [TestMethod]
        public void ReplaceAt_Throws_Expected_Exception_When_str_Is_Null()
        {
            // Arrange
            string str = null;

            // Act
            try
            {
                str.ReplaceAt(5,
                              -1,
                              "INSERT");
            }
            catch (ArgumentNullException exception) when (exception.ParamName == "str")
            {
                return;
            }

            // Assert
            Assert.Fail("The expected exception was not thrown.");
        }


        [TestMethod]
        public void ReplaceAt_Throws_Expected_Exception_When_length_Is_Negative()
        {
            // Arrange
            var str = String.Empty;

            // Act
            try
            {
                str.ReplaceAt(5,
                              -1,
                              "INSERT");
            }
            catch (ArgumentException exception) when (exception.ParamName == "length")
            {
                return;
            }

            // Assert
            Assert.Fail("The expected exception was not thrown.");
        }


        [TestMethod]
        public void Reverse_Returns_Null_When_Input_Is_Null()
        {
            // Arrange
            string inputStr = null;

            // Act
            var result = inputStr.Reverse();

            // Assert
            result.Should().BeNull();
        }


        [TestMethod]
        public void Reverse_Returns_Expected_String()
        {
            // Arrange
            var inputStr = "input string";
            var expectedResult = "gnirts tupni";

            // Act
            var result = inputStr.Reverse();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}