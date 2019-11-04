using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Random = Supertext.Base.Common.Random;

namespace Supertext.Base.Tests.Common
{
    [TestClass]
    public class RandomTests
    {
        [TestMethod]
        public void GetBytes_Returns_Collection_Of_Specified_Length()
        {
            // Arrange
            var rdm = new System.Random(4375634);
            var count = rdm.Next(300);

            // Act
            var retrievedBytes = Random.GetBytes(count);

            // Assert
            retrievedBytes.Count().Should().Be(count);
        }

        [TestMethod]
        public void GetEMail_Returns_String_In_Expected_Format()
        {
            // Arrange

            // Act
            var retrievedEmail = Random.GetEmail();
            var idxAt = retrievedEmail.IndexOf("@");
            var idxDot = retrievedEmail.LastIndexOf('.');

            // Assert
            retrievedEmail.Should().NotBeNullOrWhiteSpace();
            idxAt.Should().BeGreaterThan(0);
            idxDot.Should().BeGreaterThan(idxAt);
        }

        [TestMethod]
        public void GetInt_Is_Within_Two_Bounds()
        {
            // Arrange
            const int lowerBound = 5;
            const int upperBound = 6;

            // Act
            var retrievedInt = Random.GetInt(lowerBound, upperBound);

            // Assert
            retrievedInt.Should().Be(lowerBound);
        }

        [TestMethod]
        public void GetInt_Has_Max_Upper_Bound()
        {
            // Arrange
            const int upperBound = 0;

            // Act
            var retrievedInt = Random.GetInt(upperBound);

            // Assert
            retrievedInt.Should().Be(upperBound);
        }

        [TestMethod]
        public void GetLong_Is_Within_Two_Bounds()
        {
            // Arrange
            const int lowerBound = 5;
            const int upperBound = 6;

            // Act
            var retrievedLong = Random.GetLong(lowerBound, upperBound);

            // Assert
            retrievedLong.Should().Be(lowerBound);
        }

        [TestMethod]
        public void GetLong_Has_Max_Upper_Bound()
        {
            // Arrange
            const int upperBound = 0;

            // Act
            var retrievedLong = Random.GetLong(upperBound);

            // Assert
            retrievedLong.Should().Be(upperBound);
        }

        [TestMethod]
        public void GetString_Is_Of_Expected_Length()
        {
            // Arrange
            var expectedLength = new System.Random(5756987).Next(1000);

            // Act
            var retrievedString = Random.GetString(expectedLength);

            // Assert
            retrievedString.Length.Should().Be(expectedLength);
        }

        [TestMethod]
        public void GetString_Without_Numbers_Contains_No_Numbers()
        {
            // Arrange
            const int expectedLength = 1000;

            // Act
            var retrievedString = Random.GetString(expectedLength, false);

            // Assert
            for (var i = 0; i < 10; i++)
            {
                retrievedString.IndexOf(i.ToString()).Should().Be(-1);
            }
        }

        [TestMethod]
        public void GetUri_Returns_Valid_Uri()
        {
            // Arrange

            // Act
            var retrievedUri = Random.GetUri();

            // Assert
            Uri.IsWellFormedUriString(retrievedUri.AbsoluteUri, UriKind.Absolute);
        }
    }
}