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

            _testee = new Sha256Hasher(_saltCreator, encryptionConfig);
        }

        [TestMethod]
        public void HashLegacyToken_Returns_hashed_string_and_salt()
        {
            var expectedHashSize = 64; //SHA256 always returns a 256 bit string with 64 characters
            var legacyToken = "Generated API token";
            var expectedSalt = "ExpectedSalt";

            A.CallTo(() => _saltCreator.Generate()).Returns(expectedSalt);

            var result = _testee.HashLegacyToken(legacyToken);

            result.HashedValue.Length.Should().Be(expectedHashSize);
            result.Salt.Should().Be(expectedSalt);
        }
    }
}
