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

        [TestInitialize]
        public void TestInitialize()
        {
            var encryptionConfig = new HashingConfig()
                                   {
                                       LegacyTokenHashingPepper = nameof(HashingConfig.LegacyTokenHashingPepper),
                                       PasswordHashingPepper = nameof(HashingConfig.PasswordHashingPepper)
            };

            _saltCreator = A.Fake<ISaltGenerator>();
            _testee = new Sha256Hasher(encryptionConfig, _saltCreator);
        }

        [TestMethod]
        public void HashLegacyToken_Returns_hashed_string_and_salt()
        {
            var expectedHashSize44 = 44; //SHA256 always returns a 256 bit string with 64 characters
            var legacyToken = "Generated API token";
            var salt = "Salt";

            A.CallTo(() => _saltCreator.Generate()).Returns(salt);

            var result = _testee.HashLegacyToken(legacyToken);

            result.HashedValue.Length.Should().Be(expectedHashSize44);
            result.Salt.Should().Be(salt);
        }

        [TestMethod]
        public void HashPassword_Returns_hashed_string_and_salt()
        {
            var expectedHashSize44 = 44;
            var legacyToken = "Password123";
            var salt = "Salt";

            A.CallTo(() => _saltCreator.Generate()).Returns(salt);

            var result = _testee.HashLegacyToken(legacyToken);

            result.HashedValue.Length.Should().Be(expectedHashSize44);
            result.Salt.Should().Be(salt);
        }
    }
}
