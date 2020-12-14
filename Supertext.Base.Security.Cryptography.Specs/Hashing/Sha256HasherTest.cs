using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Security.Cryptography.Hashing;
using Supertext.Base.Security.Cryptography.Hashing.Salt;
using Supertext.Base.Security.Hashing;

namespace Supertext.Base.Security.Cryptography.Tests.Hashing
{
    [TestClass]
    public class Sha256HasherTest
    {
        private ISha256Hasher _testee;
        private ISaltGenerator _saltCreator;
        private readonly int  _expectedHashSize44 = 44; //SHA256 always returns a 256 bit string with 64 characters
        private readonly string _salt = "Salt";

        [TestInitialize]
        public void TestInitialize()
        {
            var encryptionConfig = new HashingConfig()
                                   {
                                       TokenHashingPepper = nameof(HashingConfig.TokenHashingPepper),
                                       PasswordHashingPepper = nameof(HashingConfig.PasswordHashingPepper)
            };

            _saltCreator = A.Fake<ISaltGenerator>();
            _testee = new Sha256Hasher(encryptionConfig, _saltCreator);
        }

        [TestMethod]
        public void HashToken_ValidToken_ReturnsHashingResult()
        {
            var token = "token";

            A.CallTo(() => _saltCreator.Generate()).Returns(_salt);

            var result = _testee.HashToken(token);

            result.HashedValue.Length.Should().Be(_expectedHashSize44);
            result.Salt.Should().Be(_salt);
        }

        [TestMethod]
        public void HashToken_GivenEmptyToken_ThrowsArgumentException()
        {
            var token = "";

            _testee.Invoking(testee => testee.HashToken(token)).Should()
                   .Throw<ArgumentException>().WithMessage("string is null or whitespace");
        }

        [TestMethod]
        public void HashToken_GivenNull_ThrowsArgumentException()
        {
            _testee.Invoking(testee => testee.HashToken(null)).Should()
                   .Throw<ArgumentException>().WithMessage("string is null or whitespace");
        }

        [TestMethod]
        public void HashPassword_ValidPassword_ReturnsHashingResult()
        {
            var legacyToken = "Password123";

            A.CallTo(() => _saltCreator.Generate()).Returns(_salt);

            var result = _testee.HashToken(legacyToken);

            result.HashedValue.Length.Should().Be(_expectedHashSize44);
            result.Salt.Should().Be(_salt);
        }

        [TestMethod]
        public void HashPassword_GivenEmptyToken_ThrowsArgumentException()
        {
            var password = "";

            _testee.Invoking(testee => testee.HashPassword(password)).Should()
                   .Throw<ArgumentException>().WithMessage("string is null or whitespace");
        }


        [TestMethod]
        public void HashPassword_GivenNull_ThrowsArgumentException()
        {
            _testee.Invoking(testee => testee.HashPassword(null)).Should()
                   .Throw<ArgumentException>().WithMessage("string is null or whitespace");
        }
    }
}
