using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Security.Cryptography.Hashing;
using Supertext.Base.Security.Hashing;

namespace Supertext.Base.Security.Cryptography.Tests.Hashing
{
    [TestClass]
    public class Sha256HashValidatorTest
    {
        private ISha256Hasher _sha256Hasher;
        private Sha256HashValidator _testee;
        private HashingConfig _encryptionConfig;

        [TestInitialize]
        public void TestInitialize()
        {
            _encryptionConfig = new HashingConfig()
                                   {
                                       LegacyTokenHashingPepper = nameof(HashingConfig.LegacyTokenHashingPepper),
                                       PasswordHashingPepper = nameof(HashingConfig.PasswordHashingPepper)
            };
            _sha256Hasher = A.Fake<ISha256Hasher>();
            _testee = new Sha256HashValidator(_encryptionConfig, _sha256Hasher);
        }

        [TestMethod]
        public void IsLegacyTokenValid_Returns_True_Legacy_Token_Corresponds_Hash()
        {
            var legacyToken = "Generated API token";
            var salt = "Salt";
            var expectedHash = "Expected Hash";
            var expectedHashingResult = new HashingResult
                                        {
                                            HashedValue = expectedHash,
                                            Salt = salt
            };

            A.CallTo(() => _sha256Hasher.ComputeSha256HashWithSaltAndPepper(legacyToken, salt, _encryptionConfig.LegacyTokenHashingPepper)).Returns(expectedHashingResult);

            var isValid = _testee.IsLegacyTokenValid(legacyToken, salt, expectedHash);

            isValid.Should().Be(true);
        }


        [TestMethod]
        public void IsLegacyTokenValid_Returns_False_Legacy_Token_Does_Not_Correspond_Hash()
        {
            var legacyToken = "Generated API token";
            var salt = "Salt";
            var expectedHash = "Expected Hash";
            var mismatchingHash = "Mismatching Hash";
            var mismatchingHashingResult = new HashingResult
                                           {
                                               HashedValue = mismatchingHash,
                                               Salt = salt
                                           };

            A.CallTo(() => _sha256Hasher.ComputeSha256HashWithSaltAndPepper(legacyToken, salt, _encryptionConfig.LegacyTokenHashingPepper)).Returns(mismatchingHashingResult);

            var isValid = _testee.IsLegacyTokenValid(legacyToken, salt, expectedHash);

            isValid.Should().Be(false);
        }

        [TestMethod]
        public void IsLegacyTokenValid_Returns_True_Password_Corresponds_Hash()
        {
            var password = "Password123";
            var salt = "Salt";
            var expectedHash = "Expected Hash";
            var expectedHashingResult = new HashingResult
                                        {
                                            HashedValue = expectedHash,
                                            Salt = salt
                                        };

            A.CallTo(() => _sha256Hasher.ComputeSha256HashWithSaltAndPepper(password, salt, _encryptionConfig.LegacyTokenHashingPepper)).Returns(expectedHashingResult);

            var isValid = _testee.IsLegacyTokenValid(password, salt, expectedHash);

            isValid.Should().Be(true);
        }


        [TestMethod]
        public void IsPasswordValid_Returns_False_Password_Does_Not_Correspond_Hash()
        {
            var password = "Password123";
            var salt = "Salt";
            var expectedHash = "Expected Hash";
            var mismatchingHash = "Mismatching Hash";
            var mismatchingHashingResult = new HashingResult
                                        {
                                            HashedValue = mismatchingHash,
                                            Salt = salt
                                        };

            A.CallTo(() => _sha256Hasher.ComputeSha256HashWithSaltAndPepper(password, salt, _encryptionConfig.LegacyTokenHashingPepper)).Returns(mismatchingHashingResult);

            var isValid = _testee.IsLegacyTokenValid(password, salt, expectedHash);

            isValid.Should().Be(false);
        }
    }
}
